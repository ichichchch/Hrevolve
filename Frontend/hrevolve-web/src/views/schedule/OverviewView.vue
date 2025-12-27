<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { Calendar, Clock, User, TrendCharts } from '@element-plus/icons-vue';
import { scheduleApi } from '@/api';

// 统计数据
const stats = ref({
  totalEmployees: 0,
  scheduledToday: 0,
  onDuty: 0,
  pendingApprovals: 0
});
const loading = ref(false);
const todaySchedules = ref<any[]>([]);

// 获取数据
const fetchData = async () => {
  loading.value = true;
  try {
    const [statsRes, schedulesRes] = await Promise.all([
      scheduleApi.getScheduleStats(),
      scheduleApi.getSchedules({ date: new Date().toISOString().split('T')[0] })
    ]);
    stats.value = statsRes.data;
    todaySchedules.value = schedulesRes.data.slice(0, 10);
  } catch { /* ignore */ } finally { loading.value = false; }
};

onMounted(() => fetchData());
</script>

<template>
  <div class="schedule-overview">
    <!-- 统计卡片 -->
    <el-row :gutter="16" class="stats-row">
      <el-col :xs="12" :sm="6">
        <el-card class="stat-card">
          <div class="stat-icon" style="background: linear-gradient(135deg, #D4AF37, #F4D03F)">
            <el-icon><User /></el-icon>
          </div>
          <div class="stat-info">
            <div class="stat-value">{{ stats.totalEmployees }}</div>
            <div class="stat-label">总员工数</div>
          </div>
        </el-card>
      </el-col>
      <el-col :xs="12" :sm="6">
        <el-card class="stat-card">
          <div class="stat-icon" style="background: linear-gradient(135deg, #3498DB, #5DADE2)">
            <el-icon><Calendar /></el-icon>
          </div>
          <div class="stat-info">
            <div class="stat-value">{{ stats.scheduledToday }}</div>
            <div class="stat-label">今日排班</div>
          </div>
        </el-card>
      </el-col>
      <el-col :xs="12" :sm="6">
        <el-card class="stat-card">
          <div class="stat-icon" style="background: linear-gradient(135deg, #2ECC71, #58D68D)">
            <el-icon><Clock /></el-icon>
          </div>
          <div class="stat-info">
            <div class="stat-value">{{ stats.onDuty }}</div>
            <div class="stat-label">在岗人数</div>
          </div>
        </el-card>
      </el-col>
      <el-col :xs="12" :sm="6">
        <el-card class="stat-card">
          <div class="stat-icon" style="background: linear-gradient(135deg, #E74C3C, #EC7063)">
            <el-icon><TrendCharts /></el-icon>
          </div>
          <div class="stat-info">
            <div class="stat-value">{{ stats.pendingApprovals }}</div>
            <div class="stat-label">待审批</div>
          </div>
        </el-card>
      </el-col>
    </el-row>
    
    <!-- 今日排班列表 -->
    <el-card class="today-schedules">
      <template #header>
        <span class="card-title">今日排班</span>
      </template>
      <el-table v-loading="loading" :data="todaySchedules" stripe>
        <el-table-column prop="employeeName" label="员工" width="120" />
        <el-table-column prop="departmentName" label="部门" width="120" />
        <el-table-column prop="shiftName" label="班次" width="100" />
        <el-table-column prop="startTime" label="上班时间" width="100" />
        <el-table-column prop="endTime" label="下班时间" width="100" />
        <el-table-column prop="status" label="状态" width="100">
          <template #default="{ row }">
            <el-tag :type="row.status === 'checked_in' ? 'success' : 'info'" size="small">
              {{ row.status === 'checked_in' ? '已签到' : '未签到' }}
            </el-tag>
          </template>
        </el-table-column>
      </el-table>
      <el-empty v-if="!loading && todaySchedules.length === 0" description="今日暂无排班" />
    </el-card>
  </div>
</template>

<style scoped lang="scss">
.schedule-overview {
  .stats-row { margin-bottom: 16px; }
  .stat-card {
    display: flex; align-items: center; padding: 16px;
    .stat-icon {
      width: 48px; height: 48px; border-radius: 12px;
      display: flex; align-items: center; justify-content: center;
      .el-icon { font-size: 24px; color: #fff; }
    }
    .stat-info { margin-left: 16px;
      .stat-value { font-size: 24px; font-weight: 700; color: #D4AF37; }
      .stat-label { font-size: 13px; color: rgba(255,255,255,0.65); }
    }
  }
  .today-schedules { .card-title { font-size: 16px; font-weight: 600; } }
}
</style>
