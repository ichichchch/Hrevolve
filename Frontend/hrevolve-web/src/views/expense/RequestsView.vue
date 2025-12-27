<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Plus, Search, View, Check, Close } from '@element-plus/icons-vue';
import { expenseApi } from '@/api';
import type { ExpenseRequest, ExpenseType } from '@/types';

const loading = ref(false);
const requests = ref<ExpenseRequest[]>([]);
const expenseTypes = ref<ExpenseType[]>([]);
const dialogVisible = ref(false);
const detailVisible = ref(false);
const form = ref<Partial<ExpenseRequest>>({});
const currentDetail = ref<ExpenseRequest | null>(null);
const saving = ref(false);
const searchKeyword = ref('');
const statusFilter = ref('');
const pagination = ref({ page: 1, pageSize: 20, total: 0 });

// 状态选项
const statusOptions = [
  { value: 'pending', label: '待审批', type: 'warning' },
  { value: 'approved', label: '已通过', type: 'success' },
  { value: 'rejected', label: '已拒绝', type: 'danger' },
  { value: 'paid', label: '已支付', type: 'info' },
];

// 过滤后的数据
const filteredRequests = computed(() => {
  let result = requests.value;
  if (searchKeyword.value) {
    const keyword = searchKeyword.value.toLowerCase();
    result = result.filter(r => r.title?.toLowerCase().includes(keyword) || r.employeeName?.toLowerCase().includes(keyword));
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
  form.value = { items: [], status: 'pending' };
  dialogVisible.value = true;
};

// 查看详情
const handleView = (row: ExpenseRequest) => {
  currentDetail.value = row;
  detailVisible.value = true;
};

// 审批
const handleApprove = async (row: ExpenseRequest, approved: boolean) => {
  const action = approved ? '通过' : '拒绝';
  await ElMessageBox.confirm(`确定${action}该报销申请吗？`, '提示', { type: 'warning' });
  try {
    await expenseApi.approveExpenseRequest(row.id, { approved, comment: '' });
    ElMessage.success(`已${action}`);
    fetchData();
  } catch { /* ignore */ }
};

// 保存
const handleSave = async () => {
  if (!form.value.title || !form.value.expenseTypeId) {
    ElMessage.warning('请填写必填项');
    return;
  }
  saving.value = true;
  try {
    await expenseApi.createExpenseRequest(form.value);
    ElMessage.success('提交成功');
    dialogVisible.value = false;
    fetchData();
  } catch { /* ignore */ } finally { saving.value = false; }
};

// 获取状态
const getStatusType = (status: string) => statusOptions.find(s => s.value === status)?.type || 'info';
const getStatusLabel = (status: string) => statusOptions.find(s => s.value === status)?.label || status;
const getTypeName = (typeId: string) => expenseTypes.value.find(t => t.id === typeId)?.name || '-';

onMounted(() => fetchData());
</script>

<template>
  <div class="expense-requests-view">
    <el-card>
      <template #header>
        <div class="card-header">
          <span class="card-title">报销申请</span>
          <div class="header-actions">
            <el-input v-model="searchKeyword" placeholder="搜索" :prefix-icon="Search" clearable style="width: 160px" />
            <el-select v-model="statusFilter" placeholder="状态" clearable style="width: 120px">
              <el-option v-for="s in statusOptions" :key="s.value" :label="s.label" :value="s.value" />
            </el-select>
            <el-button type="primary" :icon="Plus" @click="handleAdd">新增申请</el-button>
          </div>
        </div>
      </template>
      
      <el-table v-loading="loading" :data="filteredRequests" stripe>
        <el-table-column prop="title" label="标题" min-width="150" />
        <el-table-column prop="employeeName" label="申请人" width="100" />
        <el-table-column prop="expenseTypeId" label="类型" width="100">
          <template #default="{ row }">{{ getTypeName(row.expenseTypeId) }}</template>
        </el-table-column>
        <el-table-column prop="totalAmount" label="金额" width="100">
          <template #default="{ row }">¥{{ row.totalAmount?.toFixed(2) }}</template>
        </el-table-column>
        <el-table-column prop="status" label="状态" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.status)" size="small">{{ getStatusLabel(row.status) }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="createdAt" label="申请时间" width="160">
          <template #default="{ row }">{{ new Date(row.createdAt).toLocaleString() }}</template>
        </el-table-column>
        <el-table-column label="操作" width="150" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" size="small" @click="handleView(row)"><el-icon><View /></el-icon></el-button>
            <template v-if="row.status === 'pending'">
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
    <el-dialog v-model="dialogVisible" title="新增报销申请" width="550px">
      <el-form :model="form" label-width="80px">
        <el-form-item label="标题" required><el-input v-model="form.title" placeholder="请输入报销标题" /></el-form-item>
        <el-form-item label="类型" required>
          <el-select v-model="form.expenseTypeId" style="width: 100%">
            <el-option v-for="t in expenseTypes" :key="t.id" :label="t.name" :value="t.id" />
          </el-select>
        </el-form-item>
        <el-form-item label="金额" required><el-input-number v-model="form.totalAmount" :min="0" :precision="2" style="width: 100%" /></el-form-item>
        <el-form-item label="说明"><el-input v-model="form.description" type="textarea" :rows="3" /></el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="saving" @click="handleSave">提交</el-button>
      </template>
    </el-dialog>
    
    <!-- 详情对话框 -->
    <el-dialog v-model="detailVisible" title="报销详情" width="600px">
      <el-descriptions v-if="currentDetail" :column="2" border>
        <el-descriptions-item label="标题">{{ currentDetail.title }}</el-descriptions-item>
        <el-descriptions-item label="申请人">{{ currentDetail.employeeName }}</el-descriptions-item>
        <el-descriptions-item label="类型">{{ getTypeName(currentDetail.expenseTypeId) }}</el-descriptions-item>
        <el-descriptions-item label="金额">¥{{ currentDetail.totalAmount?.toFixed(2) }}</el-descriptions-item>
        <el-descriptions-item label="状态"><el-tag :type="getStatusType(currentDetail.status)" size="small">{{ getStatusLabel(currentDetail.status) }}</el-tag></el-descriptions-item>
        <el-descriptions-item label="申请时间">{{ new Date(currentDetail.createdAt).toLocaleString() }}</el-descriptions-item>
        <el-descriptions-item label="说明" :span="2">{{ currentDetail.description || '-' }}</el-descriptions-item>
      </el-descriptions>
    </el-dialog>
  </div>
</template>

<style scoped lang="scss">
.expense-requests-view {
  .card-header { display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; gap: 12px;
    .card-title { font-size: 16px; font-weight: 600; }
    .header-actions { display: flex; gap: 12px; }
  }
  .pagination-wrapper { margin-top: 16px; display: flex; justify-content: flex-end; }
}
</style>
