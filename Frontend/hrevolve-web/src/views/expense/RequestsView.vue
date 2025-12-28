<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Plus, Search, View, Check, Close } from '@element-plus/icons-vue';
import { expenseApi } from '@/api';
import type { ExpenseRequest, ExpenseType } from '@/types';
import dayjs from 'dayjs';

const { t } = useI18n();

const loading = ref(false);
const requests = ref<ExpenseRequest[]>([]);
const expenseTypes = ref<ExpenseType[]>([]);
const dialogVisible = ref(false);
const detailVisible = ref(false);
const currentDetail = ref<ExpenseRequest | null>(null);
const saving = ref(false);
const searchKeyword = ref('');
const statusFilter = ref('');
const pagination = ref({ page: 1, pageSize: 20, total: 0 });

// 表单数据
const form = ref({
  expenseTypeId: '',
  amount: 0,
  expenseDate: dayjs().format('YYYY-MM-DD'),
  description: '',
});

// 状态选项 - 使用 computed 以支持响应式翻译
const statusOptions = computed(() => [
  { value: 'Pending', label: t('expense.statusPending'), type: 'warning' },
  { value: 'Approved', label: t('expense.statusApproved'), type: 'success' },
  { value: 'Rejected', label: t('expense.statusRejected'), type: 'danger' },
  { value: 'Paid', label: t('expense.statusPaid'), type: 'info' },
]);

// 过滤后的数据
const filteredRequests = computed(() => {
  let result = requests.value;
  if (searchKeyword.value) {
    const keyword = searchKeyword.value.toLowerCase();
    result = result.filter(r => 
      r.description?.toLowerCase().includes(keyword) || 
      r.employeeName?.toLowerCase().includes(keyword)
    );
  }
  if (statusFilter.value) result = result.filter(r => r.status === statusFilter.value);
  return result;
});

// 获取数据
const fetchData = async () => {
  loading.value = true;
  try {
    const [requestsRes, typesRes] = await Promise.all([
      expenseApi.getExpenseRequests({ page: pagination.value.page, pageSize: pagination.value.pageSize }),
      expenseApi.getExpenseTypes()
    ]);
    requests.value = requestsRes.data.items || requestsRes.data;
    pagination.value.total = requestsRes.data.total || requests.value.length;
    expenseTypes.value = typesRes.data;
  } catch { /* ignore */ } finally { loading.value = false; }
};

// 新增
const handleAdd = () => {
  form.value = {
    expenseTypeId: '',
    amount: 0,
    expenseDate: dayjs().format('YYYY-MM-DD'),
    description: '',
  };
  dialogVisible.value = true;
};

// 查看详情
const handleView = (row: ExpenseRequest) => {
  currentDetail.value = row;
  detailVisible.value = true;
};

// 审批
const handleApprove = async (row: ExpenseRequest, approved: boolean) => {
  const action = approved ? t('expense.actionApprove') : t('expense.actionReject');
  await ElMessageBox.confirm(
    t('expense.confirmApproval', { action }),
    t('common.confirm'),
    { type: 'warning' }
  );
  try {
    await expenseApi.approveExpenseRequest(row.id, { approved, comment: '' });
    ElMessage.success(t('expense.approvalSuccess', { action }));
    fetchData();
  } catch { /* ignore */ }
};

// 保存
const handleSave = async () => {
  if (!form.value.description || !form.value.expenseTypeId) {
    ElMessage.warning(t('expense.validation.fillRequired'));
    return;
  }
  saving.value = true;
  try {
    await expenseApi.createExpenseRequest({
      expenseTypeId: form.value.expenseTypeId,
      amount: form.value.amount,
      expenseDate: form.value.expenseDate,
      description: form.value.description,
    });
    ElMessage.success(t('expense.submitSuccess'));
    dialogVisible.value = false;
    fetchData();
  } catch { /* ignore */ } finally { saving.value = false; }
};

// 获取状态
const getStatusType = (status: string) => statusOptions.value.find(s => s.value === status)?.type || 'info';
const getStatusLabel = (status: string) => statusOptions.value.find(s => s.value === status)?.label || status;
const getTypeName = (typeId: string) => expenseTypes.value.find(t => t.id === typeId)?.name || '-';

onMounted(() => fetchData());
</script>

<template>
  <div class="expense-requests-view">
    <el-card>
      <template #header>
        <div class="card-header">
          <span class="card-title">{{ t('expense.requestTitle') }}</span>
          <div class="header-actions">
            <el-input v-model="searchKeyword" :placeholder="t('common.search')" :prefix-icon="Search" clearable style="width: 160px" />
            <el-select v-model="statusFilter" :placeholder="t('common.status')" clearable style="width: 120px">
              <el-option v-for="s in statusOptions" :key="s.value" :label="s.label" :value="s.value" />
            </el-select>
            <el-button type="primary" :icon="Plus" @click="handleAdd">{{ t('expense.addRequest') }}</el-button>
          </div>
        </div>
      </template>
      
      <el-table v-loading="loading" :data="filteredRequests" stripe>
        <el-table-column prop="description" :label="t('expense.title')" min-width="150" show-overflow-tooltip />
        <el-table-column prop="employeeName" :label="t('expense.applicant')" width="100" />
        <el-table-column prop="expenseTypeId" :label="t('expense.type')" width="100">
          <template #default="{ row }">{{ getTypeName(row.expenseTypeId) }}</template>
        </el-table-column>
        <el-table-column prop="amount" :label="t('expense.amount')" width="100">
          <template #default="{ row }">¥{{ row.amount?.toFixed(2) }}</template>
        </el-table-column>
        <el-table-column prop="status" :label="t('common.status')" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.status)" size="small">{{ getStatusLabel(row.status) }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="createdAt" :label="t('expense.applyTime')" width="160">
          <template #default="{ row }">{{ new Date(row.createdAt).toLocaleString() }}</template>
        </el-table-column>
        <el-table-column :label="t('common.actions')" width="150" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" size="small" @click="handleView(row)"><el-icon><View /></el-icon></el-button>
            <template v-if="row.status === 'Pending'">
              <el-button link type="success" size="small" @click="handleApprove(row, true)"><el-icon><Check /></el-icon></el-button>
              <el-button link type="danger" size="small" @click="handleApprove(row, false)"><el-icon><Close /></el-icon></el-button>
            </template>
          </template>
        </el-table-column>
      </el-table>
      
      <div class="pagination-wrapper">
        <el-pagination v-model:current-page="pagination.page" :page-size="pagination.pageSize" :total="pagination.total" layout="total, prev, pager, next" @current-change="fetchData" />
      </div>
    </el-card>
    
    <!-- 新增对话框 -->
    <el-dialog v-model="dialogVisible" :title="t('expense.addRequestDialog')" width="550px">
      <el-form :model="form" label-width="80px">
        <el-form-item :label="t('expense.title')" required>
          <el-input v-model="form.description" :placeholder="t('expense.placeholder.title')" />
        </el-form-item>
        <el-form-item :label="t('expense.type')" required>
          <el-select v-model="form.expenseTypeId" style="width: 100%">
            <el-option v-for="type in expenseTypes" :key="type.id" :label="type.name" :value="type.id" />
          </el-select>
        </el-form-item>
        <el-form-item :label="t('expense.amount')" required>
          <el-input-number v-model="form.amount" :min="0" :precision="2" style="width: 100%" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">{{ t('common.cancel') }}</el-button>
        <el-button type="primary" :loading="saving" @click="handleSave">{{ t('expense.submit') }}</el-button>
      </template>
    </el-dialog>
    
    <!-- 详情对话框 -->
    <el-dialog v-model="detailVisible" :title="t('expense.detailDialog')" width="600px">
      <el-descriptions v-if="currentDetail" :column="2" border>
        <el-descriptions-item :label="t('expense.title')">{{ currentDetail.description }}</el-descriptions-item>
        <el-descriptions-item :label="t('expense.applicant')">{{ currentDetail.employeeName }}</el-descriptions-item>
        <el-descriptions-item :label="t('expense.type')">{{ getTypeName(currentDetail.expenseTypeId) }}</el-descriptions-item>
        <el-descriptions-item :label="t('expense.amount')">¥{{ currentDetail.amount?.toFixed(2) }}</el-descriptions-item>
        <el-descriptions-item :label="t('common.status')">
          <el-tag :type="getStatusType(currentDetail.status)" size="small">{{ getStatusLabel(currentDetail.status) }}</el-tag>
        </el-descriptions-item>
        <el-descriptions-item :label="t('expense.applyTime')">{{ new Date(currentDetail.createdAt).toLocaleString() }}</el-descriptions-item>
      </el-descriptions>
    </el-dialog>
  </div>
</template>

<style scoped lang="scss">
.expense-requests-view {
  .card-header { 
    display: flex; 
    justify-content: space-between; 
    align-items: center; 
    flex-wrap: wrap; 
    gap: 12px;
    .card-title { font-size: 16px; font-weight: 600; }
    .header-actions { display: flex; gap: 12px; }
  }
  .pagination-wrapper { margin-top: 16px; display: flex; justify-content: flex-end; }
}
</style>
