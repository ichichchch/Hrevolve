<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Plus, Edit, Delete } from '@element-plus/icons-vue';
import { settingsApi } from '@/api';
import type { ApprovalFlow } from '@/types';

const loading = ref(false);
const flows = ref<ApprovalFlow[]>([]);
const dialogVisible = ref(false);
const dialogTitle = ref('');
const form = ref<Partial<ApprovalFlow>>({});
const saving = ref(false);

// 流程类型
const flowTypes = [
  { value: 'leave', label: '请假审批' },
  { value: 'expense', label: '报销审批' },
  { value: 'overtime', label: '加班审批' },
  { value: 'attendance', label: '考勤异常' },
];

const fetchData = async () => {
  loading.value = true;
  try {
    const res = await settingsApi.getApprovalFlows();
    flows.value = res.data;
  } catch { /* ignore */ } finally { loading.value = false; }
};

const handleAdd = () => {
  form.value = { type: 'leave', isActive: true, steps: [{ order: 1, approverType: 'supervisor', approverIds: [] }] };
  dialogTitle.value = '新增审批流程';
  dialogVisible.value = true;
};

const handleEdit = (item: ApprovalFlow) => {
  form.value = JSON.parse(JSON.stringify(item));
  dialogTitle.value = '编辑审批流程';
  dialogVisible.value = true;
};

const handleDelete = async (item: ApprovalFlow) => {
  await ElMessageBox.confirm(`确定删除"${item.name}"吗？`, '提示', { type: 'warning' });
  try {
    await settingsApi.deleteApprovalFlow(item.id);
    ElMessage.success('删除成功');
    fetchData();
  } catch { /* ignore */ }
};

const handleSave = async () => {
  if (!form.value.name || !form.value.type) { ElMessage.warning('请填写必填项'); return; }
  saving.value = true;
  try {
    if (form.value.id) {
      await settingsApi.updateApprovalFlow(form.value.id, form.value);
    } else {
      await settingsApi.createApprovalFlow(form.value);
    }
    ElMessage.success('保存成功');
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

const getTypeName = (type: string) => flowTypes.find(t => t.value === type)?.label || type;

onMounted(() => fetchData());
</script>

<template>
  <div class="approval-flows-view">
    <el-card>
      <template #header>
        <div class="card-header">
          <span class="card-title">审批流程管理</span>
          <el-button type="primary" :icon="Plus" @click="handleAdd">新增流程</el-button>
        </div>
      </template>
      
      <el-table v-loading="loading" :data="flows" stripe>
        <el-table-column prop="name" label="名称" min-width="150" />
        <el-table-column prop="type" label="类型" width="120">
          <template #default="{ row }"><el-tag size="small">{{ getTypeName(row.type) }}</el-tag></template>
        </el-table-column>
        <el-table-column prop="steps" label="审批层级" width="100">
          <template #default="{ row }">{{ row.steps?.length || 0 }}级</template>
        </el-table-column>
        <el-table-column prop="isActive" label="状态" width="80">
          <template #default="{ row }">
            <el-tag :type="row.isActive ? 'success' : 'danger'" size="small">{{ row.isActive ? '启用' : '禁用' }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="description" label="描述" min-width="200" show-overflow-tooltip />
        <el-table-column label="操作" width="120" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" size="small" @click="handleEdit(row)"><el-icon><Edit /></el-icon></el-button>
            <el-button link type="danger" size="small" @click="handleDelete(row)"><el-icon><Delete /></el-icon></el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
    
    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="600px">
      <el-form :model="form" label-width="100px">
        <el-form-item label="名称" required><el-input v-model="form.name" placeholder="请输入流程名称" /></el-form-item>
        <el-form-item label="类型" required>
          <el-select v-model="form.type" style="width: 100%">
            <el-option v-for="t in flowTypes" :key="t.value" :label="t.label" :value="t.value" />
          </el-select>
        </el-form-item>
        <el-form-item label="描述"><el-input v-model="form.description" type="textarea" :rows="2" /></el-form-item>
        <el-form-item label="审批步骤">
          <div class="steps-list">
            <div v-for="(step, index) in form.steps" :key="index" class="step-item">
              <span class="step-order">第{{ step.order }}级</span>
              <el-select v-model="step.approverType" style="width: 150px">
                <el-option label="直属上级" value="supervisor" />
                <el-option label="部门负责人" value="department_head" />
                <el-option label="指定人员" value="specific" />
                <el-option label="HR" value="hr" />
              </el-select>
              <el-button link type="danger" @click="removeStep(index)"><el-icon><Delete /></el-icon></el-button>
            </div>
            <el-button type="primary" link @click="addStep"><el-icon><Plus /></el-icon> 添加步骤</el-button>
          </div>
        </el-form-item>
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
