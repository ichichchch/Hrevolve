<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { payrollApi } from '@/api';
import type { PayrollPeriod } from '@/types';
import dayjs from 'dayjs';

const { t } = useI18n();
const periods = ref<PayrollPeriod[]>([]);
const loading = ref(false);

const fetchPeriods = async () => {
  loading.value = true;
  try { 
    const res = await payrollApi.getPeriods(); 
    periods.value = res.data; 
  } catch { /* ignore */ } finally { loading.value = false; }
};

const formatDate = (d: string) => dayjs(d).format('YYYY-MM-DD');
const getStatusType = (s: string) => ({ Open: 'success', Closed: 'info', Locked: 'danger' }[s] || 'info');

onMounted(() => fetchPeriods());
</script>

<template>
  <div class="periods-view">
    <el-card>
      <template #header><span>{{ t('payrollAdmin.periods') }}</span></template>
      <el-table :data="periods" v-loading="loading" stripe>
        <el-table-column prop="name" :label="t('payrollAdmin.periodName')" width="150" />
        <el-table-column prop="year" :label="t('payrollAdmin.year')" width="80" />
        <el-table-column prop="month" :label="t('payrollAdmin.month')" width="80" />
        <el-table-column prop="startDate" :label="t('payrollAdmin.startDate')" width="120"><template #default="{ row }">{{ formatDate(row.startDate) }}</template></el-table-column>
        <el-table-column prop="endDate" :label="t('payrollAdmin.endDate')" width="120"><template #default="{ row }">{{ formatDate(row.endDate) }}</template></el-table-column>
        <el-table-column prop="status" :label="t('common.status')" width="100"><template #default="{ row }"><el-tag :type="getStatusType(row.status)" size="small">{{ row.status }}</el-tag></template></el-table-column>
      </el-table>
    </el-card>
  </div>
</template>
