<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { leaveApi } from '@/api';
import type { LeaveRequest } from '@/types';
import dayjs from 'dayjs';

const { t } = useI18n();
const requests = ref<LeaveRequest[]>([]);
const loading = ref(false);

const fetchRequests = async () => {
  loading.value = true;
  try { 
    const res = await leaveApi.getRequests({ page: 1, pageSize: 50 }); 
    requests.value = res.data.items; 
  } catch { /* ignore */ } finally { loading.value = false; }
};

const formatDate = (d: string) => dayjs(d).format('YYYY-MM-DD');
const getStatusType = (s: string) => ({ Pending: 'warning', Approved: 'success', Rejected: 'danger', Cancelled: 'info' }[s] || 'info');

// 状态标签映射 - 使用 computed 实现响应式翻译
const statusLabels = computed(() => ({
  Pending: t('leave.statusPending'),
  Approved: t('leave.statusApproved'),
  Rejected: t('leave.statusRejected'),
  Cancelled: t('leave.statusCancelled'),
} as Record<string, string>));

onMounted(() => fetchRequests());
</script>

<template>
  <div class="requests-view">
    <el-card>
      <template #header><span>{{ t('menu.leaveRequests') }}</span></template>
      <el-table :data="requests" v-loading="loading" stripe>
        <el-table-column prop="employeeName" :label="t('schedule.employee')" width="100" />
        <el-table-column prop="leaveTypeName" :label="t('leave.leaveType')" width="100" />
        <el-table-column prop="startDate" :label="t('leave.startDate')" width="120"><template #default="{ row }">{{ formatDate(row.startDate) }}</template></el-table-column>
        <el-table-column prop="endDate" :label="t('leave.endDate')" width="120"><template #default="{ row }">{{ formatDate(row.endDate) }}</template></el-table-column>
        <el-table-column prop="days" :label="t('leave.days')" width="80" />
        <el-table-column prop="reason" :label="t('leave.reason')" />
        <el-table-column prop="status" :label="t('common.status')" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.status)" size="small">{{ statusLabels[row.status] || row.status }}</el-tag>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>
