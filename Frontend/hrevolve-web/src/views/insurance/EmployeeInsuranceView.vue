<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Plus, Search, Delete } from '@element-plus/icons-vue';
import { insuranceApi } from '@/api';
import type { EmployeeInsurance, InsurancePlan } from '@/types';

const { t } = useI18n();
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
  await ElMessageBox.confirm(t('insurance.confirmTerminate'), t('common.confirm'), { type: 'warning' });
  try {
    await insuranceApi.terminateEmployeeInsurance(item.id);
    ElMessage.success(t('common.success'));
    fetchData();
  } catch { /* ignore */ }
};

const handleSave = async () => {
  if (!form.value.employeeId || !form.value.planId) {
    ElMessage.warning(t('tax.fillRequired'));
    return;
  }
  saving.value = true;
  try {
    await insuranceApi.enrollEmployeeInsurance(form.value);
    ElMessage.success(t('common.success'));
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
          <span class="card-title">{{ t('insurance.employeeInsurance') }}</span>
          <div class="header-actions">
            <el-input v-model="searchKeyword" :placeholder="t('common.search')" :prefix-icon="Search" clearable style="width: 180px" />
            <el-button type="primary" :icon="Plus" @click="handleAdd">{{ t('insurance.addEnrollment') }}</el-button>
          </div>
        </div>
      </template>
      
      <el-table v-loading="loading" :data="insurances" stripe>
        <el-table-column prop="employeeName" :label="t('schedule.employee')" width="120" />
        <el-table-column prop="planId" :label="t('insurance.insurancePlan')" min-width="150">
          <template #default="{ row }">{{ getPlanName(row.planId) }}</template>
        </el-table-column>
        <el-table-column prop="startDate" :label="t('insurance.effectiveDate')" width="120" />
        <el-table-column prop="endDate" :label="t('insurance.terminationDate')" width="120">
          <template #default="{ row }">{{ row.endDate || '-' }}</template>
        </el-table-column>
        <el-table-column prop="premium" :label="t('insurance.premium')" width="100">
          <template #default="{ row }">¥{{ row.premium }}</template>
        </el-table-column>
        <el-table-column prop="status" :label="t('common.status')" width="100">
          <template #default="{ row }">
            <el-tag :type="row.status === 'active' ? 'success' : 'info'" size="small">{{ row.status === 'active' ? t('insurance.statusActive') : t('insurance.statusTerminated') }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column :label="t('common.actions')" width="100" fixed="right">
          <template #default="{ row }">
            <el-button v-if="row.status === 'active'" link type="danger" size="small" @click="handleTerminate(row)">
              <el-icon><Delete /></el-icon> {{ t('insurance.terminate') }}
            </el-button>
          </template>
        </el-table-column>
      </el-table>
      
      <div class="pagination-wrapper">
        <el-pagination v-model:current-page="pagination.page" :page-size="pagination.pageSize" :total="pagination.total" layout="total, prev, pager, next" @current-change="fetchData" />
      </div>
    </el-card>
    
    <el-dialog v-model="dialogVisible" :title="t('insurance.addEnrollment')" width="500px">
      <el-form :model="form" label-width="100px">
        <el-form-item :label="t('schedule.employee')" required>
          <el-input v-model="form.employeeId" :placeholder="t('insurance.enterEmployeeId')" />
        </el-form-item>
        <el-form-item :label="t('insurance.insurancePlan')" required>
          <el-select v-model="form.planId" style="width: 100%">
            <el-option v-for="p in plans" :key="p.id" :label="p.name" :value="p.id" />
          </el-select>
        </el-form-item>
        <el-form-item :label="t('insurance.effectiveDate')" required>
          <el-date-picker v-model="form.startDate" type="date" value-format="YYYY-MM-DD" style="width: 100%" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">{{ t('common.cancel') }}</el-button>
        <el-button type="primary" :loading="saving" @click="handleSave">{{ t('common.save') }}</el-button>
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
