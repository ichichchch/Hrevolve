<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Plus, Edit, Delete } from '@element-plus/icons-vue';
import { expenseApi } from '@/api';
import type { ExpenseType } from '@/types';

const { t } = useI18n();

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
  dialogTitle.value = t('expense.addTypeDialog');
  dialogVisible.value = true;
};

// 编辑
const handleEdit = (item: ExpenseType) => {
  form.value = { ...item };
  dialogTitle.value = t('expense.editTypeDialog');
  dialogVisible.value = true;
};

// 删除
const handleDelete = async (item: ExpenseType) => {
  await ElMessageBox.confirm(
    t('expense.confirmDelete', { name: item.name }),
    t('common.confirm'),
    { type: 'warning' }
  );
  try {
    await expenseApi.deleteExpenseType(item.id);
    ElMessage.success(t('expense.deleteSuccess'));
    fetchData();
  } catch { /* ignore */ }
};

// 保存
const handleSave = async () => {
  if (!form.value.name || !form.value.code) {
    ElMessage.warning(t('expense.validation.fillRequired'));
    return;
  }
  saving.value = true;
  try {
    if (form.value.id) {
      await expenseApi.updateExpenseType(form.value.id, form.value);
    } else {
      await expenseApi.createExpenseType(form.value);
    }
    ElMessage.success(t('expense.saveSuccess'));
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
          <span class="card-title">{{ t('expense.typeManagement') }}</span>
          <el-button type="primary" :icon="Plus" @click="handleAdd">{{ t('expense.addType') }}</el-button>
        </div>
      </template>
      
      <el-table v-loading="loading" :data="types" stripe>
        <el-table-column prop="code" :label="t('expense.code')" width="120" />
        <el-table-column prop="name" :label="t('expense.name')" min-width="150" />
        <el-table-column prop="maxAmount" :label="t('expense.maxAmount')" width="120">
          <template #default="{ row }">{{ row.maxAmount ? `¥${row.maxAmount}` : t('expense.noLimit') }}</template>
        </el-table-column>
        <el-table-column prop="requiresReceipt" :label="t('expense.requiresReceipt')" width="100">
          <template #default="{ row }">
            <el-tag :type="row.requiresReceipt ? 'warning' : 'info'" size="small">{{ row.requiresReceipt ? t('common.yes') : t('common.no') }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="isActive" :label="t('common.status')" width="80">
          <template #default="{ row }">
            <el-tag :type="row.isActive ? 'success' : 'danger'" size="small">{{ row.isActive ? t('expense.enabled') : t('expense.disabled') }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="description" :label="t('expense.description')" min-width="200" show-overflow-tooltip />
        <el-table-column :label="t('common.actions')" width="120" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" size="small" @click="handleEdit(row)"><el-icon><Edit /></el-icon></el-button>
            <el-button link type="danger" size="small" @click="handleDelete(row)"><el-icon><Delete /></el-icon></el-button>
          </template>
        </el-table-column>
      </el-table>
      
      <el-empty v-if="!loading && types.length === 0" :description="t('expense.noTypes')" />
    </el-card>
    
    <!-- 编辑对话框 -->
    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="500px">
      <el-form :model="form" label-width="100px">
        <el-form-item :label="t('expense.code')" required><el-input v-model="form.code" :placeholder="t('expense.placeholder.code')" /></el-form-item>
        <el-form-item :label="t('expense.name')" required><el-input v-model="form.name" :placeholder="t('expense.placeholder.name')" /></el-form-item>
        <el-form-item :label="t('expense.maxAmount')"><el-input-number v-model="form.maxAmount" :min="0" :precision="2" :placeholder="t('expense.placeholder.maxAmount')" style="width: 100%" /></el-form-item>
        <el-form-item :label="t('expense.requiresReceipt')"><el-switch v-model="form.requiresReceipt" /></el-form-item>
        <el-form-item :label="t('expense.description')"><el-input v-model="form.description" type="textarea" :rows="2" /></el-form-item>
        <el-form-item :label="t('common.status')"><el-switch v-model="form.isActive" :active-text="t('expense.enabled')" :inactive-text="t('expense.disabled')" /></el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">{{ t('common.cancel') }}</el-button>
        <el-button type="primary" :loading="saving" @click="handleSave">{{ t('common.save') }}</el-button>
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
