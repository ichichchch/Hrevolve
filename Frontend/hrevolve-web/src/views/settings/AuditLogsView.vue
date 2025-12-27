<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { Search, Download, View } from '@element-plus/icons-vue';
import { settingsApi } from '@/api';
import type { AuditLog } from '@/types';

const loading = ref(false);
const logs = ref<AuditLog[]>([]);
const detailVisible = ref(false);
const currentLog = ref<AuditLog | null>(null);
const searchKeyword = ref('');
const actionFilter = ref('');
const dateRange = ref<[Date, Date] | null>(null);
const pagination = ref({ page: 1, pageSize: 20, total: 0 });

// 操作类型
const actionTypes = [
  { value: 'create', label: '创建', type: 'success' },
  { value: 'update', label: '更新', type: 'warning' },
  { value: 'delete', label: '删除', type: 'danger' },
  { value: 'login', label: '登录', type: 'info' },
  { value: 'logout', label: '登出', type: 'info' },
  { value: 'export', label: '导出', type: '' },
];

const fetchData = async () => {
  loading.value = true;
  try {
    const params: any = { page: pagination.value.page, pageSize: pagination.value.pageSize };
    if (actionFilter.value) params.action = actionFilter.value;
    if (dateRange.value) {
      params.startDate = dateRange.value[0].toISOString().split('T')[0];
      params.endDate = dateRange.value[1].toISOString().split('T')[0];
    }
    const res = await settingsApi.getAuditLogs(params);
    logs.value = res.data.items || res.data;
    pagination.value.total = res.data.total || logs.value.length;
  } catch { /* ignore */ } finally { loading.value = false; }
};

const handleView = (log: AuditLog) => {
  currentLog.value = log;
  detailVisible.value = true;
};

const handleExport = async () => {
  try {
    await settingsApi.exportAuditLogs({
      action: actionFilter.value || undefined,
      startDate: dateRange.value?.[0].toISOString().split('T')[0],
      endDate: dateRange.value?.[1].toISOString().split('T')[0],
    });
  } catch { /* ignore */ }
};

const getActionType = (action: string) => actionTypes.find(a => a.value === action)?.type || 'info';
const getActionLabel = (action: string) => actionTypes.find(a => a.value === action)?.label || action;

// 过滤
const filteredLogs = computed(() => {
  if (!searchKeyword.value) return logs.value;
  const keyword = searchKeyword.value.toLowerCase();
  return logs.value.filter(l => 
    l.userName?.toLowerCase().includes(keyword) || 
    l.entityType?.toLowerCase().includes(keyword) ||
    l.description?.toLowerCase().includes(keyword)
  );
});

onMounted(() => fetchData());
</script>

<template>
  <div class="audit-logs-view">
    <el-card>
      <template #header>
        <div class="card-header">
          <span class="card-title">审计日志</span>
          <div class="header-actions">
            <el-date-picker v-model="dateRange" type="daterange" start-placeholder="开始日期" end-placeholder="结束日期" style="width: 240px" @change="fetchData" />
            <el-select v-model="actionFilter" placeholder="操作类型" clearable style="width: 120px" @change="fetchData">
              <el-option v-for="a in actionTypes" :key="a.value" :label="a.label" :value="a.value" />
            </el-select>
            <el-input v-model="searchKeyword" placeholder="搜索" :prefix-icon="Search" clearable style="width: 160px" />
            <el-button :icon="Download" @click="handleExport">导出</el-button>
          </div>
        </div>
      </template>
      
      <el-table v-loading="loading" :data="filteredLogs" stripe>
        <el-table-column prop="createdAt" label="时间" width="170">
          <template #default="{ row }">{{ new Date(row.createdAt).toLocaleString() }}</template>
        </el-table-column>
        <el-table-column prop="userName" label="用户" width="120" />
        <el-table-column prop="action" label="操作" width="100">
          <template #default="{ row }">
            <el-tag :type="getActionType(row.action)" size="small">{{ getActionLabel(row.action) }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="entityType" label="对象类型" width="120" />
        <el-table-column prop="description" label="描述" min-width="200" show-overflow-tooltip />
        <el-table-column prop="ipAddress" label="IP地址" width="130" />
        <el-table-column label="操作" width="80" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" size="small" @click="handleView(row)"><el-icon><View /></el-icon></el-button>
          </template>
        </el-table-column>
      </el-table>
      
      <div class="pagination-wrapper">
        <el-pagination v-model:current-page="pagination.page" :page-size="pagination.pageSize" :total="pagination.total" layout="total, prev, pager, next" @current-change="fetchData" />
      </div>
    </el-card>
    
    <!-- 详情对话框 -->
    <el-dialog v-model="detailVisible" title="日志详情" width="600px">
      <el-descriptions v-if="currentLog" :column="2" border>
        <el-descriptions-item label="时间">{{ new Date(currentLog.createdAt).toLocaleString() }}</el-descriptions-item>
        <el-descriptions-item label="用户">{{ currentLog.userName }}</el-descriptions-item>
        <el-descriptions-item label="操作"><el-tag :type="getActionType(currentLog.action)" size="small">{{ getActionLabel(currentLog.action) }}</el-tag></el-descriptions-item>
        <el-descriptions-item label="对象类型">{{ currentLog.entityType }}</el-descriptions-item>
        <el-descriptions-item label="对象ID">{{ currentLog.entityId }}</el-descriptions-item>
        <el-descriptions-item label="IP地址">{{ currentLog.ipAddress }}</el-descriptions-item>
        <el-descriptions-item label="描述" :span="2">{{ currentLog.description }}</el-descriptions-item>
        <el-descriptions-item label="变更详情" :span="2">
          <pre v-if="currentLog.changes" class="changes-json">{{ JSON.stringify(currentLog.changes, null, 2) }}</pre>
          <span v-else>-</span>
        </el-descriptions-item>
      </el-descriptions>
    </el-dialog>
  </div>
</template>

<style scoped lang="scss">
.audit-logs-view {
  .card-header { display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; gap: 12px;
    .card-title { font-size: 16px; font-weight: 600; }
    .header-actions { display: flex; gap: 12px; flex-wrap: wrap; }
  }
  .pagination-wrapper { margin-top: 16px; display: flex; justify-content: flex-end; }
  .changes-json { background: rgba(0,0,0,0.3); padding: 8px; border-radius: 4px; font-size: 12px; max-height: 200px; overflow: auto; margin: 0; }
}
</style>
