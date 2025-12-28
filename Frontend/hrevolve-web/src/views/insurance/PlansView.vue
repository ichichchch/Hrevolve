<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Plus, Edit, Delete } from '@element-plus/icons-vue';
import { insuranceApi } from '@/api';
import type { InsurancePlan } from '@/types';

const { t } = useI18n();
const loading = ref(false);
const plans = ref<InsurancePlan[]>([]);
const dialogVisible = ref(false);
const dialogTitle = ref('');
const form = ref<Partial<InsurancePlan>>({});
const saving = ref(false);

// 保险类型 - 使用 computed 实现响应式翻译
const planTypes = computed(() => [
  { value: 'health', label: t('insurance.typeMedical') },
  { value: 'life', label: t('insurance.typeLife') },
  { value: 'accident', label: t('insurance.typeAccident') },
  { value: 'pension', label: t('insurance.typePension') },
]);

const fetchData = async () => {
  loading.value = true;
  try {
    const res = await insuranceApi.getInsurancePlans();
    plans.value = res.data;
  } catch { /* ignore */ } finally { loading.value = false; }
};

const handleAdd = () => {
  form.value = { type: 'health', isActive: true };
  dialogTitle.value = t('insurance.newPlan');
  dialogVisible.value = true;
};

const handleEdit = (item: InsurancePlan) => {
  form.value = { ...item };
  dialogTitle.value = t('insurance.editPlan');
  dialogVisible.value = true;
};

const handleDelete = async (item: InsurancePlan) => {
  await ElMessageBox.confirm(t('expense.confirmDelete', { name: item.name }), t('common.confirm'), { type: 'warning' });
  try {
    await insuranceApi.deleteInsurancePlan(item.id);
    ElMessage.success(t('common.success'));
    fetchData();
  } catch { /* ignore */ }
};

const handleSave = async () => {
  if (!form.value.name || !form.value.type) {
    ElMessage.warning(t('tax.fillRequired'));
    return;
  }
  saving.value = true;
  try {
    if (form.value.id) {
      await insuranceApi.updateInsurancePlan(form.value.id, form.value);
    } else {
      await insuranceApi.createInsurancePlan(form.value);
    }
    ElMessage.success(t('common.success'));
    dialogVisible.value = false;
    fetchData();
  } catch { /* ignore */ } finally { saving.value = false; }
};

const getTypeName = (type: string) => planTypes.value.find(t => t.value === type)?.label || type;

onMounted(() => fetchData());
</script>

<template>
  <div class="insurance-plans-view">
    <el-card>
      <template #header>
        <div class="card-header">
          <span class="card-title">{{ t('insurance.planManagement') }}</span>
          <el-button type="primary" :icon="Plus" @click="handleAdd">{{ t('insurance.addPlan') }}</el-button>
        </div>
      </template>
      
      <el-row :gutter="16" v-loading="loading">
        <el-col v-for="item in plans" :key="item.id" :xs="24" :sm="12" :md="8">
          <el-card class="plan-card" shadow="hover">
            <div class="plan-header">
              <span class="plan-name">{{ item.name }}</span>
              <el-tag :type="item.isActive ? 'success' : 'danger'" size="small">{{ item.isActive ? t('settings.enabled') : t('settings.disabled') }}</el-tag>
            </div>
            <div class="plan-type"><el-tag size="small">{{ getTypeName(item.type) }}</el-tag></div>
            <div class="plan-info">
              <div class="info-item"><span class="label">{{ t('insurance.premium') }}:</span><span class="value">¥{{ item.premium }}{{ t('insurance.perMonth') }}</span></div>
              <div class="info-item"><span class="label">{{ t('insurance.coverage') }}:</span><span class="value">¥{{ item.coverage?.toLocaleString() }}</span></div>
            </div>
            <div class="plan-desc">{{ item.description || t('common.noData') }}</div>
            <div class="plan-actions">
              <el-button link type="primary" size="small" @click="handleEdit(item)"><el-icon><Edit /></el-icon> {{ t('common.edit') }}</el-button>
              <el-button link type="danger" size="small" @click="handleDelete(item)"><el-icon><Delete /></el-icon> {{ t('common.delete') }}</el-button>
            </div>
          </el-card>
        </el-col>
      </el-row>
      
      <el-empty v-if="!loading && plans.length === 0" :description="t('common.noData')" />
    </el-card>
    
    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="500px">
      <el-form :model="form" label-width="80px">
        <el-form-item :label="t('insurance.planName')" required><el-input v-model="form.name" /></el-form-item>
        <el-form-item :label="t('insurance.planType')" required>
          <el-select v-model="form.type" style="width: 100%">
            <el-option v-for="pt in planTypes" :key="pt.value" :label="pt.label" :value="pt.value" />
          </el-select>
        </el-form-item>
        <el-form-item :label="t('insurance.premium')"><el-input-number v-model="form.premium" :min="0" :precision="2" style="width: 100%" /><span style="margin-left: 8px">{{ t('insurance.perMonth') }}</span></el-form-item>
        <el-form-item :label="t('insurance.coverage')"><el-input-number v-model="form.coverage" :min="0" style="width: 100%" /></el-form-item>
        <el-form-item :label="t('settings.description')"><el-input v-model="form.description" type="textarea" :rows="2" /></el-form-item>
        <el-form-item :label="t('common.status')"><el-switch v-model="form.isActive" :active-text="t('settings.enabled')" :inactive-text="t('settings.disabled')" /></el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">{{ t('common.cancel') }}</el-button>
        <el-button type="primary" :loading="saving" @click="handleSave">{{ t('common.save') }}</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<style scoped lang="scss">
.insurance-plans-view {
  .card-header { display: flex; justify-content: space-between; align-items: center;
    .card-title { font-size: 16px; font-weight: 600; }
  }
  .plan-card { margin-bottom: 16px;
    .plan-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 8px;
      .plan-name { font-size: 16px; font-weight: 600; color: #D4AF37; }
    }
    .plan-type { margin-bottom: 12px; }
    .plan-info { display: flex; gap: 16px; margin-bottom: 8px;
      .info-item { font-size: 13px;
        .label { color: rgba(255,255,255,0.5); margin-right: 4px; }
        .value { font-weight: 500; }
      }
    }
    .plan-desc { font-size: 13px; color: rgba(255,255,255,0.65); margin-bottom: 12px; }
    .plan-actions { border-top: 1px solid rgba(212,175,55,0.2); padding-top: 12px; display: flex; gap: 8px; }
  }
}
</style>
