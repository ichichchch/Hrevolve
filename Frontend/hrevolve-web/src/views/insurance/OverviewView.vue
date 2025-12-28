<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { Document, User, Money, TrendCharts } from '@element-plus/icons-vue';
import { insuranceApi } from '@/api';

const { t } = useI18n();
const loading = ref(false);
const stats = ref({ totalPlans: 0, enrolledEmployees: 0, monthlyPremium: 0, pendingClaims: 0 });
const recentEnrollments = ref<any[]>([]);

const fetchData = async () => {
  loading.value = true;
  try {
    const [statsRes, enrollmentsRes] = await Promise.all([
      insuranceApi.getInsuranceStats(),
      insuranceApi.getEmployeeInsurances({ pageSize: 10 })
    ]);
    stats.value = statsRes.data;
    recentEnrollments.value = enrollmentsRes.data.items || enrollmentsRes.data;
  } catch { /* ignore */ } finally { loading.value = false; }
};

onMounted(() => fetchData());
</script>

<template>
  <div class="insurance-overview">
    <el-row :gutter="16" class="stats-row">
      <el-col :xs="12" :sm="6">
        <el-card class="stat-card">
          <div class="stat-icon" style="background: linear-gradient(135deg, #D4AF37, #F4D03F)"><el-icon><Document /></el-icon></div>
          <div class="stat-info"><div class="stat-value">{{ stats.totalPlans }}</div><div class="stat-label">{{ t('insurance.insurancePlan') }}</div></div>
        </el-card>
      </el-col>
      <el-col :xs="12" :sm="6">
        <el-card class="stat-card">
          <div class="stat-icon" style="background: linear-gradient(135deg, #3498DB, #5DADE2)"><el-icon><User /></el-icon></div>
          <div class="stat-info"><div class="stat-value">{{ stats.enrolledEmployees }}</div><div class="stat-label">{{ t('menu.employeeInsurance') }}</div></div>
        </el-card>
      </el-col>
      <el-col :xs="12" :sm="6">
        <el-card class="stat-card">
          <div class="stat-icon" style="background: linear-gradient(135deg, #2ECC71, #58D68D)"><el-icon><Money /></el-icon></div>
          <div class="stat-info"><div class="stat-value">¥{{ stats.monthlyPremium?.toLocaleString() }}</div><div class="stat-label">{{ t('insurance.premium') }}</div></div>
        </el-card>
      </el-col>
      <el-col :xs="12" :sm="6">
        <el-card class="stat-card">
          <div class="stat-icon" style="background: linear-gradient(135deg, #E74C3C, #EC7063)"><el-icon><TrendCharts /></el-icon></div>
          <div class="stat-info"><div class="stat-value">{{ stats.pendingClaims }}</div><div class="stat-label">{{ t('dashboard.pendingApprovals') }}</div></div>
        </el-card>
      </el-col>
    </el-row>
    
    <el-card>
      <template #header><span class="card-title">{{ t('insurance.recentEnrollments') }}</span></template>
      <el-table v-loading="loading" :data="recentEnrollments" stripe>
        <el-table-column prop="employeeName" :label="t('schedule.employee')" width="120" />
        <el-table-column prop="planName" :label="t('insurance.insurancePlan')" min-width="150" />
        <el-table-column prop="startDate" :label="t('insurance.effectiveDate')" width="120" />
        <el-table-column prop="premium" :label="t('insurance.premium')" width="100"><template #default="{ row }">¥{{ row.premium }}</template></el-table-column>
        <el-table-column prop="status" :label="t('common.status')" width="100">
          <template #default="{ row }"><el-tag :type="row.status === 'active' ? 'success' : 'info'" size="small">{{ row.status === 'active' ? t('insurance.statusActive') : t('insurance.statusTerminated') }}</el-tag></template>
        </el-table-column>
      </el-table>
      <el-empty v-if="!loading && recentEnrollments.length === 0" :description="t('common.noData')" />
    </el-card>
  </div>
</template>

<style scoped lang="scss">
.insurance-overview {
  .stats-row { margin-bottom: 16px; }
  .stat-card { display: flex; align-items: center; padding: 16px;
    .stat-icon { width: 48px; height: 48px; border-radius: 12px; display: flex; align-items: center; justify-content: center;
      .el-icon { font-size: 24px; color: #fff; }
    }
    .stat-info { margin-left: 16px;
      .stat-value { font-size: 24px; font-weight: 700; color: #D4AF37; }
      .stat-label { font-size: 13px; color: rgba(255,255,255,0.65); }
    }
  }
  .card-title { font-size: 16px; font-weight: 600; }
}
</style>
