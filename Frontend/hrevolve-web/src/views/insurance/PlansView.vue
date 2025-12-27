<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Plus, Edit, Delete } from '@element-plus/icons-vue';
import { insuranceApi } from '@/api';
import type { InsurancePlan } from '@/types';

const loading = ref(false);
const plans = ref<InsurancePlan[]>([]);
const dialogVisible = ref(false);
const dialogTitle = ref('');
const form = ref<Partial<InsurancePlan>>({});
const saving = ref(false);

// 保险类型
const planTypes = [
  { value: 'health', label: '医疗保险' },
  { value: 'life', label: '人寿保险' },
  { value: 'accident', label: '意外保险' },
  { value: 'pension', label: '养老保险' },
  { value: 'unemployment', label: '失业保险' },
];

const fetchData = async () => {
  loading.value = true;
  try {
    const res = await insuranceApi.getInsurancePlans();
    plans.value = res.data;
  } catch { /* ignore */ } finally { loading.value = false; }
};

const handleAdd = () => {
  form.value = { type: 'health', isActive: true };
  dialogTitle.value = '新增保险方案';
  dialogVisible.value = true;
};

const handleEdit = (item: InsurancePlan) => {
  form.value = { ...item };
  dialogTitle.value = '编辑保险方案';
  dialogVisible.value = true;
};

const handleDelete = async (item: InsurancePlan) => {
  await ElMessageBox.confirm(`确定删除"${item.name}"吗？`, '提示', { type: 'warning' });
  try {
    await insuranceApi.deleteInsurancePlan(item.id);
    ElMessage.success('删除成功');
    fetchData();
  } catch { /* ignore */ }
};

const handleSave = async () => {
  if (!form.value.name || !form.value.type) {
    ElMessage.warning('请填写必填项');
    return;
  }
  saving.value = true;
  try {
    if (form.value.id) {
      await insuranceApi.updateInsurancePlan(form.value.id, form.value);
    } else {
      await insuranceApi.createInsurancePlan(form.value);
    }
    ElMessage.success('保存成功');
    dialogVisible.value = false;
    fetchData();
  } catch { /* ignore */ } finally { saving.value = false; }
};

const getTypeName = (type: string) => planTypes.find(t => t.value === type)?.label || type;

onMounted(() => fetchData());
</script>

<template>
  <div class="insurance-plans-view">
    <el-card>
      <template #header>
        <div class="card-header">
          <span class="card-title">保险方案管理</span>
          <el-button type="primary" :icon="Plus" @click="handleAdd">新增方案</el-button>
        </div>
      </template>
      
      <el-row :gutter="16" v-loading="loading">
        <el-col v-for="item in plans" :key="item.id" :xs="24" :sm="12" :md="8">
          <el-card class="plan-card" shadow="hover">
            <div class="plan-header">
              <span class="plan-name">{{ item.name }}</span>
              <el-tag :type="item.isActive ? 'success' : 'danger'" size="small">{{ item.isActive ? '启用' : '禁用' }}</el-tag>
            </div>
            <div class="plan-type"><el-tag size="small">{{ getTypeName(item.type) }}</el-tag></div>
            <div class="plan-info">
              <div class="info-item"><span class="label">保费:</span><span class="value">¥{{ item.premium }}/月</span></div>
              <div class="info-item"><span class="label">保额:</span><span class="value">¥{{ item.coverage?.toLocaleString() }}</span></div>
            </div>
            <div class="plan-desc">{{ item.description || '暂无描述' }}</div>
            <div class="plan-actions">
              <el-button link type="primary" size="small" @click="handleEdit(item)"><el-icon><Edit /></el-icon> 编辑</el-button>
              <el-button link type="danger" size="small" @click="handleDelete(item)"><el-icon><Delete /></el-icon> 删除</el-button>
            </div>
          </el-card>
        </el-col>
      </el-row>
      
      <el-empty v-if="!loading && plans.length === 0" description="暂无保险方案" />
    </el-card>
    
    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="500px">
      <el-form :model="form" label-width="80px">
        <el-form-item label="名称" required><el-input v-model="form.name" placeholder="请输入方案名称" /></el-form-item>
        <el-form-item label="类型" required>
          <el-select v-model="form.type" style="width: 100%">
            <el-option v-for="t in planTypes" :key="t.value" :label="t.label" :value="t.value" />
          </el-select>
        </el-form-item>
        <el-form-item label="保费"><el-input-number v-model="form.premium" :min="0" :precision="2" style="width: 100%" /><span style="margin-left: 8px">/月</span></el-form-item>
        <el-form-item label="保额"><el-input-number v-model="form.coverage" :min="0" style="width: 100%" /></el-form-item>
        <el-form-item label="描述"><el-input v-model="form.description" type="textarea" :rows="2" /></el-form-item>
        <el-form-item label="状态"><el-switch v-model="form.isActive" active-text="启用" inactive-text="禁用" /></el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="saving" @click="handleSave">保存</el-button>
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
