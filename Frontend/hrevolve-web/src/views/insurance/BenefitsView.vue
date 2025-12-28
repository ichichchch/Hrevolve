<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Plus, Edit, Delete } from '@element-plus/icons-vue';
import { insuranceApi } from '@/api';
import type { Benefit } from '@/types';

const { t } = useI18n();
const loading = ref(false);
const benefits = ref<Benefit[]>([]);
const dialogVisible = ref(false);
const dialogTitle = ref('');
const form = ref<Partial<Benefit>>({});
const saving = ref(false);

// 福利类型 - 使用 computed 实现响应式翻译
const benefitTypes = computed(() => [
  { value: 'meal', label: t('insurance.typeMeal') },
  { value: 'transport', label: t('insurance.typeTransport') },
  { value: 'housing', label: t('insurance.typeHousing') },
  { value: 'other', label: t('insurance.typeOther') },
]);

const fetchData = async () => {
  loading.value = true;
  try {
    const res = await insuranceApi.getBenefits();
    benefits.value = res.data;
  } catch { /* ignore */ } finally { loading.value = false; }
};

const handleAdd = () => {
  form.value = { type: 'meal', isActive: true };
  dialogTitle.value = t('insurance.newBenefit');
  dialogVisible.value = true;
};

const handleEdit = (item: Benefit) => {
  form.value = { ...item };
  dialogTitle.value = t('insurance.editBenefit');
  dialogVisible.value = true;
};

const handleDelete = async (item: Benefit) => {
  await ElMessageBox.confirm(t('expense.confirmDelete', { name: item.name }), t('common.confirm'), { type: 'warning' });
  try {
    await insuranceApi.deleteBenefit(item.id);
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
      await insuranceApi.updateBenefit(form.value.id, form.value);
    } else {
      await insuranceApi.createBenefit(form.value);
    }
    ElMessage.success(t('common.success'));
    dialogVisible.value = false;
    fetchData();
  } catch { /* ignore */ } finally { saving.value = false; }
};

const getTypeName = (type: string) => benefitTypes.value.find(bt => bt.value === type)?.label || type;

onMounted(() => fetchData());
</script>

<template>
  <div class="benefits-view">
    <el-card>
      <template #header>
        <div class="card-header">
          <span class="card-title">{{ t('insurance.benefitManagement') }}</span>
          <el-button type="primary" :icon="Plus" @click="handleAdd">{{ t('insurance.addBenefit') }}</el-button>
        </div>
      </template>
      
      <el-table v-loading="loading" :data="benefits" stripe>
        <el-table-column prop="name" :label="t('insurance.benefitName')" min-width="150" />
        <el-table-column prop="type" :label="t('insurance.benefitType')" width="120">
          <template #default="{ row }"><el-tag size="small">{{ getTypeName(row.type) }}</el-tag></template>
        </el-table-column>
        <el-table-column prop="amount" :label="t('insurance.amount')" width="120">
          <template #default="{ row }">¥{{ row.amount }}{{ t('insurance.perMonth') }}</template>
        </el-table-column>
        <el-table-column prop="isActive" :label="t('common.status')" width="80">
          <template #default="{ row }">
            <el-tag :type="row.isActive ? 'success' : 'danger'" size="small">{{ row.isActive ? t('settings.enabled') : t('settings.disabled') }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="description" :label="t('settings.description')" min-width="200" show-overflow-tooltip />
        <el-table-column :label="t('common.actions')" width="120" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" size="small" @click="handleEdit(row)"><el-icon><Edit /></el-icon></el-button>
            <el-button link type="danger" size="small" @click="handleDelete(row)"><el-icon><Delete /></el-icon></el-button>
          </template>
        </el-table-column>
      </el-table>
      
      <el-empty v-if="!loading && benefits.length === 0" :description="t('common.noData')" />
    </el-card>
    
    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="500px">
      <el-form :model="form" label-width="80px">
        <el-form-item :label="t('insurance.benefitName')" required><el-input v-model="form.name" :placeholder="t('insurance.enterBenefitName')" /></el-form-item>
        <el-form-item :label="t('insurance.benefitType')" required>
          <el-select v-model="form.type" style="width: 100%">
            <el-option v-for="bt in benefitTypes" :key="bt.value" :label="bt.label" :value="bt.value" />
          </el-select>
        </el-form-item>
        <el-form-item :label="t('insurance.amount')"><el-input-number v-model="form.amount" :min="0" :precision="2" style="width: 100%" /><span style="margin-left: 8px">{{ t('insurance.perMonth') }}</span></el-form-item>
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
.benefits-view {
  .card-header { display: flex; justify-content: space-between; align-items: center;
    .card-title { font-size: 16px; font-weight: 600; }
  }
}
</style>
