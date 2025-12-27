<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Plus, Search, Delete } from '@element-plus/icons-vue';
import { insuranceApi } from '@/api';
import type { EmployeeInsurance, InsurancePlan } from '@/types';

const loading = ref(false);
const insurances = ref<EmployeeInsurance[]>([]);
const plans = ref<InsurancePlan[]>([]);
const dialogVisible = ref(false);
const form = ref<Partial<EmployeeInsurance>>({});
const saving = ref(false);
const searchKeyword = ref('');
const pagination = ref({ page: 1, pageSize: 20, total: 0 });

const fetchData = async () => {
  loading.value = true;
  try {
    const [insurancesRes, plansRes] = await Promise.all([
      insuranceApi.getEmployeeInsurances({ page: pagination.value.page, pageSize: pagination.value.pageSize }),
      insuranceApi.getInsurancePlans()
    ]);
    insurances.value = insurancesRes.data.items || insurancesRes.data;
    pagination.value.total = insurancesRes.data.total || insurances.value.length;
    plans.value = plansRes.data;
  } catch { /* ignore */ } finally { loading.value = false; }
};

const handleAdd = () => {
  form.value = { status: 'active' };
  dialogVisible.value = true;
};

const handleTerminate = async (item: EmployeeInsurance) => {
  await ElMessageBox.confirm(`确定终止该员工的保险吗？`, '提示', { type: 'warning' });
  try {
    await insuranceApi.terminateEmployeeInsurance(item.id);
    ElMessage.success('已终止');
    fetchData();
  } catch { /* ignore */ }
};

const handleSave = async () => {
  if (!form.value.employeeId || !form.value.planId) {
    ElMessage.warning('请填写必填项');
    return;
  }
  saving.value = true;
  try {
    await insuranceApi.enrollEmployeeInsurance(form.value);
    ElMessage.success('参保成功');
    dialogVisible.value = false;
    fetchData();
  } catch { /* ignore */ } finally { saving.value = false; }
};

const getPlanName = (planId: string) => plans.value.find(p => p.id === planId)?.name || '-';

onMounted(() => fetchData());
</script>

<template>
  <div class="employee-insurance-view">
    <el-card>
      <template #header>
        <div class="card-header">
          <span class="card-title">员工参保管理</span>
          <div class="header-actions">
            <el-input v-model="searchKeyword" placeholder="搜索员工" :prefix-icon="Search" clearable style="width: 180px" />
            <el-button type="primary" :icon="Plus" @click="handleAdd">新增参保</el-button>
          </div>
        </div>
      </template>
      
      <el-table v-loading="loading" :data="insurances" stripe>
        <el-table-column prop="employeeName" label="员工" width="120" />
        <el-table-column prop="planId" label="保险方案" min-width="150">
          <template #default="{ row }">{{ getPlanName(row.planId) }}</template>
        </el-table-column>
        <el-table-column prop="startDate" label="生效日期" width="120" />
        <el-table-column prop="endDate" label="终止日期" width="120">
          <template #default="{ row }">{{ row.endDate || '-' }}</template>
        </el-table-column>
        <el-table-column prop="premium" label="保费" width="100">
          <template #default="{ row }">¥{{ row.premium }}</template>
        </el-table-column>
        <el-table-column prop="status" label="状态" width="100">
          <template #default="{ row }">
            <el-tag :type="row.status === 'active' ? 'success' : 'info'" size="small">{{ row.status === 'active' ? '生效中' : '已终止' }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="100" fixed="right">
          <template #default="{ row }">
            <el-button v-if="row.status === 'active'" link type="danger" size="small" @click="handleTerminate(row)">
              <el-icon><Delete /></el-icon> 终止
            </el-button>
          </template>
        </el-table-column>
      </el-table>
      
      <div class="pagination-wrapper">
        <el-pagination v-model:current-page="pagination.page" :page-size="pagination.pageSize" :total="pagination.total" layout="total, prev, pager, next" @current-change="fetchData" />
      </div>
    </el-card>
    
    <el-dialog v-model="dialogVisible" title="新增参保" width="500px">
      <el-form :model="form" label-width="100px">
        <el-form-item label="员工" required>
          <el-input v-model="form.employeeId" placeholder="请输入员工ID" />
        </el-form-item>
        <el-form-item label="保险方案" required>
          <el-select v-model="form.planId" style="width: 100%">
            <el-option v-for="p in plans" :key="p.id" :label="p.name" :value="p.id" />
          </el-select>
        </el-form-item>
        <el-form-item label="生效日期" required>
          <el-date-picker v-model="form.startDate" type="date" value-format="YYYY-MM-DD" style="width: 100%" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="saving" @click="handleSave">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<style scoped lang="scss">
.employee-insurance-view {
  .card-header { display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; gap: 12px;
    .card-title { font-size: 16px; font-weight: 600; }
    .header-actions { display: flex; gap: 12px; }
  }
  .pagination-wrapper { margin-top: 16px; display: flex; justify-content: flex-end; }
}
</style>
