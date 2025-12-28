<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Plus, Edit, Delete } from '@element-plus/icons-vue';
import { settingsApi } from '@/api';
import type { ApprovalFlow } from '@/types';

const { t } = useI18n();
const loading = ref(false);
const flows = ref<ApprovalFlow[]>([]);
const dialogVisible = ref(false);
const dialogTitle = ref('');
const form = ref<Partial<ApprovalFlow>>({});
const saving = ref(false);

// 流程类型 - 使用 computed 实现响应式翻译
const flowTypes = computed(() => [
  { value: 'leave', label: t('settings.flowTypeLeave') },
  { value: 'expense', label: t('settings.flowTypeExpense') },
  { value: 'overtime', label: t('settings.flowTypeOvertime') },
  { value: 'attendance', label: t('settings.flowTypeAttendance') },
]);

// 审批人类型 - 使用 computed 实现响应式翻译
const approverTypes = computed(() => [
  { value: 'supervisor', label: t('settings.approverSupervisor') },
  { value: 'department_head', label: t('settings.approverDepartmentHead') },
  { value: 'specific', label: t('settings.approverSpecific') },
  { value: 'hr', label: t('settings.approverHR') },
]);

const fetchData = async () => {
  loading.value = true;
  try {
    const res = await settingsApi.getApprovalFlows();
    flows.value = res.data;
  } catch { /* ignore */ } finally { loading.value = false; }
};

const handleAdd = () => {
  form.value = { type: 'leave', isActive: true, steps: [{ order: 1, approverType: 'supervisor', approverIds: [] }] };
  dialogTitle.value = t('settings.newFlow');
  dialogVisible.value = true;
};

const handleEdit = (item: ApprovalFlow) => {
  form.value = JSON.parse(JSON.stringify(item));
  dialogTitle.value = t('settings.editFlow');
  dialogVisible.value = true;
};

const handleDelete = async (item: ApprovalFlow) => {
  await ElMessageBox.confirm(t('settings.confirmDeleteFlow', { name: item.name }), t('common.confirm'), { type: 'warning' });
  try {
    await settingsApi.deleteApprovalFlow(item.id);
    ElMessage.success(t('common.success'));
    fetchData();
  } catch { /* ignore */ }
};

const handleSave = async () => {
  if (!form.value.name || !form.value.type) { ElMessage.warning(t('tax.fillRequired')); return; }
  saving.value = true;
  try {
    if (form.value.id) {
      await settingsApi.updateApprovalFlow(form.value.id, form.value);
    } else {
      await settingsApi.createApprovalFlow(form.value);
    }
    ElMessage.success(t('common.success'));
    dialogVisible.value = false;
    fetchData();
  } catch { /* ignore */ } finally { saving.value = false; }
};

// 添加审批步骤
const addStep = () => {
  if (!form.value.steps) form.value.steps = [];
  form.value.steps.push({ order: form.value.steps.length + 1, approverType: 'supervisor', approverIds: [] });
};

// 删除审批步骤
const removeStep = (index: number) => {
  form.value.steps?.splice(index, 1);
  form.value.steps?.forEach((s, i) => s.order = i + 1);
};

const getTypeName = (type: string) => flowTypes.value.find(t => t.value === type)?.label || type;

onMounted(() => fetchData());
</script>

<template>
  <div class="approval-flows-view">
    <el-card>
      <template #header>
        <div class="card-header">
          <span class="card-title">{{ t('settings.approvalFlows') }}</span>
          <el-button type="primary" :icon="Plus" @click="handleAdd">{{ t('settings.addFlow') }}</el-button>
        </div>
      </template>
      
      <el-table v-loading="loading" :data="flows" stripe>
        <el-table-column prop="name" :label="t('settings.name')" min-width="150" />
        <el-table-column prop="type" :label="t('settings.flowType')" width="120">
          <template #default="{ row }"><el-tag size="small">{{ getTypeName(row.type) }}</el-tag></template>
        </el-table-column>
        <el-table-column prop="steps" :label="t('settings.approvalSteps')" width="100">
          <template #default="{ row }">{{ row.steps?.length || 0 }}{{ t('settings.stepLevel', { level: '' }).replace('{level}', '') }}</template>
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
    </el-card>
    
    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="600px">
      <el-form :model="form" label-width="100px">
        <el-form-item :label="t('settings.name')" required><el-input v-model="form.name" :placeholder="t('settings.enterFlowName')" /></el-form-item>
        <el-form-item :label="t('settings.flowType')" required>
          <el-select v-model="form.type" style="width: 100%">
            <el-option v-for="ft in flowTypes" :key="ft.value" :label="ft.label" :value="ft.value" />
          </el-select>
        </el-form-item>
        <el-form-item :label="t('settings.description')"><el-input v-model="form.description" type="textarea" :rows="2" /></el-form-item>
        <el-form-item :label="t('settings.approvalSteps')">
          <div class="steps-list">
            <div v-for="(step, index) in form.steps" :key="index" class="step-item">
              <span class="step-order">{{ t('settings.stepLevel', { level: step.order }) }}</span>
              <el-select v-model="step.approverType" style="width: 150px">
                <el-option v-for="at in approverTypes" :key="at.value" :label="at.label" :value="at.value" />
              </el-select>
              <el-button link type="danger" @click="removeStep(index)"><el-icon><Delete /></el-icon></el-button>
            </div>
            <el-button type="primary" link @click="addStep"><el-icon><Plus /></el-icon> {{ t('settings.addStep') }}</el-button>
          </div>
        </el-form-item>
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
.approval-flows-view {
  .card-header { display: flex; justify-content: space-between; align-items: center;
    .card-title { font-size: 16px; font-weight: 600; }
  }
  .steps-list {
    .step-item { display: flex; align-items: center; gap: 12px; margin-bottom: 8px;
      .step-order { font-size: 13px; color: #D4AF37; min-width: 50px; }
    }
  }
}
</style>
