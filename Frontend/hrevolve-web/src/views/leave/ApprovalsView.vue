<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { ElMessage } from 'element-plus';
import { leaveApi } from '@/api';
import type { LeaveRequest } from '@/types';
import dayjs from 'dayjs';

const { t } = useI18n();
const requests = ref<LeaveRequest[]>([]);
const loading = ref(false);

const fetchRequests = async () => {
  loading.value = true;
  try { 
    const res = await leaveApi.getPendingApprovals({ page: 1, pageSize: 50 }); 
    requests.value = res.data.items; 
  } catch { /* ignore */ } finally { loading.value = false; }
};

const handleApprove = async (id: string, approved: boolean) => {
  try { 
    await leaveApi.approveRequest(id, { approved }); 
    ElMessage.success(approved ? t('leaveAdmin.approved') : t('leaveAdmin.rejected')); 
    fetchRequests(); 
  } catch { 
    ElMessage.error(t('leaveAdmin.operationFailed')); 
  }
};

const formatDate = (d: string) => dayjs(d).format('YYYY-MM-DD');

onMounted(() => fetchRequests());
</script>

<template>
  <div class="approvals-view">
    <el-card>
      <template #header><span>{{ t('leaveAdmin.pendingApprovals') }}</span></template>
      <el-table :data="requests" v-loading="loading" stripe>
        <el-table-column prop="employeeName" :label="t('schedule.employee')" width="100" />
        <el-table-column prop="leaveTypeName" :label="t('leave.leaveType')" width="100" />
        <el-table-column prop="startDate" :label="t('leave.startDate')" width="120"><template #default="{ row }">{{ formatDate(row.startDate) }}</template></el-table-column>
        <el-table-column prop="endDate" :label="t('leave.endDate')" width="120"><template #default="{ row }">{{ formatDate(row.endDate) }}</template></el-table-column>
        <el-table-column prop="days" :label="t('leave.days')" width="80" />
        <el-table-column prop="reason" :label="t('leave.reason')" />
        <el-table-column :label="t('common.actions')" width="150">
          <template #default="{ row }">
            <el-button type="success" size="small" @click="handleApprove(row.id, true)">{{ t('leaveAdmin.approve') }}</el-button>
            <el-button type="danger" size="small" @click="handleApprove(row.id, false)">{{ t('leaveAdmin.reject') }}</el-button>
          </template>
        </el-table-column>
      </el-table>
      <el-empty v-if="!loading && requests.length === 0" :description="t('leaveAdmin.noPendingApprovals')" />
    </el-card>
  </div>
</template>
