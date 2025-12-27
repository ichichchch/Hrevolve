<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Plus, Edit, Delete } from '@element-plus/icons-vue';
import { expenseApi } from '@/api';
import type { ExpenseType } from '@/types';

const loading = ref(false);
const types = ref<ExpenseType[]>([]);
const dialogVisible = ref(false);
const dialogTitle = ref('');
const form = ref<Partial<ExpenseType>>({});
const saving = ref(false);

// 获取数据
const fetchData = async () => {
  loading.value = true;
  try {
    const res = await expenseApi.getExpenseTypes();
    types.value = res.data;
  } catch { /* ignore */ } finally { loading.value = false; }
};

// 新增
const handleAdd = () => {
  form.value = { isActive: true, requiresReceipt: true };
  dialogTitle.value = '新增报销类型';
  dialogVisible.value = true;
};

// 编辑
const handleEdit = (item: ExpenseType) => {
  form.value = { ...item };
  dialogTitle.value = '编辑报销类型';
  dialogVisible.value = true;
};

// 删除
const handleDelete = async (item: ExpenseType) => {
  await ElMessageBox.confirm(`确定删除"${item.name}"吗？`, '提示', { type: 'warning' });
  try {
    await expenseApi.deleteExpenseType(item.id);
    ElMessage.success('删除成功');
    fetchData();
  } catch { /* ignore */ }
};

// 保存
const handleSave = async () => {
  if (!form.value.name || !form.value.code) {
    ElMessage.warning('请填写必填项');
    return;
  }
  saving.value = true;
  try {
    if (form.value.id) {
      await expenseApi.updateExpenseType(form.value.id, form.value);
    } else {
      await expenseApi.createExpenseType(form.value);
    }
    ElMessage.success('保存成功');
    dialogVisible.value = false;
    fetchData();
  } catch { /* ignore */ } finally { saving.value = false; }
};

onMounted(() => fetchData());
</script>

<template>
  <div class="expense-types-view">
    <el-card>
      <template #header>
        <div class="card-header">
          <span class="card-title">报销类型管理</span>
          <el-button type="primary" :icon="Plus" @click="handleAdd">新增类型</el-button>
        </div>
      </template>
      
      <el-table v-loading="loading" :data="types" stripe>
        <el-table-column prop="code" label="编码" width="120" />
        <el-table-column prop="name" label="名称" min-width="150" />
        <el-table-column prop="maxAmount" label="最高限额" width="120">
          <template #default="{ row }">{{ row.maxAmount ? `¥${row.maxAmount}` : '无限制' }}</template>
        </el-table-column>
        <el-table-column prop="requiresReceipt" label="需要发票" width="100">
          <template #default="{ row }">
            <el-tag :type="row.requiresReceipt ? 'warning' : 'info'" size="small">{{ row.requiresReceipt ? '是' : '否' }}</el-tag>
          </template>
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
      
      <el-empty v-if="!loading && types.length === 0" description="暂无报销类型" />
    </el-card>
    
    <!-- 编辑对话框 -->
    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="500px">
      <el-form :model="form" label-width="100px">
        <el-form-item label="编码" required><el-input v-model="form.code" placeholder="如：TRAVEL" /></el-form-item>
        <el-form-item label="名称" required><el-input v-model="form.name" placeholder="如：差旅费" /></el-form-item>
        <el-form-item label="最高限额"><el-input-number v-model="form.maxAmount" :min="0" :precision="2" placeholder="留空为无限制" style="width: 100%" /></el-form-item>
        <el-form-item label="需要发票"><el-switch v-model="form.requiresReceipt" /></el-form-item>
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
.expense-types-view {
  .card-header { display: flex; justify-content: space-between; align-items: center;
    .card-title { font-size: 16px; font-weight: 600; }
  }
}
</style>
