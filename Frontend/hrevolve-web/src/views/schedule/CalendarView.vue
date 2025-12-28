<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { ArrowLeft, ArrowRight } from '@element-plus/icons-vue';
import { scheduleApi } from '@/api';

const { t } = useI18n();
const loading = ref(false);
const currentDate = ref(new Date());
const schedules = ref<any[]>([]);

// 当前月份的日期
const calendarDays = computed(() => {
  const year = currentDate.value.getFullYear();
  const month = currentDate.value.getMonth();
  const firstDay = new Date(year, month, 1);
  const lastDay = new Date(year, month + 1, 0);
  const days: { date: Date; isCurrentMonth: boolean }[] = [];
  
  // 上月补齐
  const startDay = firstDay.getDay() || 7;
  for (let i = startDay - 1; i > 0; i--) {
    const d = new Date(year, month, 1 - i);
    days.push({ date: d, isCurrentMonth: false });
  }
  // 当月
  for (let i = 1; i <= lastDay.getDate(); i++) {
    days.push({ date: new Date(year, month, i), isCurrentMonth: true });
  }
  // 下月补齐
  const remaining = 42 - days.length;
  for (let i = 1; i <= remaining; i++) {
    days.push({ date: new Date(year, month + 1, i), isCurrentMonth: false });
  }
  return days;
});

// 星期标题（响应式）
const weekdayHeaders = computed(() => {
  const keys = ['mon', 'tue', 'wed', 'thu', 'fri', 'sat', 'sun'];
  const prefix = t('weekdays.prefix');
  return keys.map(key => prefix + t(`weekdays.${key}`));
});

// 格式化
const formatDate = (d: Date) => d.toISOString().split('T')[0];
const formatMonth = (d: Date) => t('calendar.yearMonth', { year: d.getFullYear(), month: d.getMonth() + 1 });

// 获取某天的排班数量
const getDayScheduleCount = (date: Date) => {
  const dateStr = formatDate(date);
  return schedules.value.filter(s => s.date === dateStr).length;
};

// 切换月份
const changeMonth = (delta: number) => {
  const d = new Date(currentDate.value);
  d.setMonth(d.getMonth() + delta);
  currentDate.value = d;
  fetchData();
};

// 获取数据
const fetchData = async () => {
  loading.value = true;
  try {
    const year = currentDate.value.getFullYear();
    const month = currentDate.value.getMonth();
    const startDate = formatDate(new Date(year, month, 1));
    const endDate = formatDate(new Date(year, month + 1, 0));
    const res = await scheduleApi.getSchedules({ page: 1, pageSize: 1000, startDate, endDate });
    schedules.value = Array.isArray(res.data) ? res.data : (res.data as any).items || [];
  } catch { /* ignore */ } finally { loading.value = false; }
};

// 是否今天
const isToday = (date: Date) => formatDate(date) === formatDate(new Date());

onMounted(() => fetchData());
</script>

<template>
  <div class="calendar-view">
    <el-card>
      <template #header>
        <div class="card-header">
          <span class="card-title">{{ t('calendar.title') }}</span>
          <div class="month-nav">
            <el-button :icon="ArrowLeft" @click="changeMonth(-1)" />
            <span class="month-label">{{ formatMonth(currentDate) }}</span>
            <el-button :icon="ArrowRight" @click="changeMonth(1)" />
          </div>
        </div>
      </template>
      
      <div class="calendar-grid" v-loading="loading">
        <div class="weekday-header">
          <div v-for="day in weekdayHeaders" :key="day" class="weekday">{{ day }}</div>
        </div>
        <div class="days-grid">
          <div
            v-for="(item, idx) in calendarDays"
            :key="idx"
            class="day-cell"
            :class="{ 'other-month': !item.isCurrentMonth, 'today': isToday(item.date) }"
          >
            <span class="day-number">{{ item.date.getDate() }}</span>
            <div v-if="getDayScheduleCount(item.date) > 0" class="schedule-count">
              {{ t('calendar.scheduleCount', { count: getDayScheduleCount(item.date) }) }}
            </div>
          </div>
        </div>
      </div>
    </el-card>
  </div>
</template>

<style scoped lang="scss">
.calendar-view {
  .card-header { display: flex; justify-content: space-between; align-items: center;
    .card-title { font-size: 16px; font-weight: 600; }
    .month-nav { display: flex; align-items: center; gap: 12px;
      .month-label { font-size: 16px; font-weight: 500; min-width: 100px; text-align: center; }
    }
  }
  .calendar-grid {
    .weekday-header { display: grid; grid-template-columns: repeat(7, 1fr); gap: 4px; margin-bottom: 8px;
      .weekday { text-align: center; padding: 8px; font-size: 13px; color: rgba(255,255,255,0.65); background: rgba(212,175,55,0.1); border-radius: 4px; }
    }
    .days-grid { display: grid; grid-template-columns: repeat(7, 1fr); gap: 4px;
      .day-cell { min-height: 80px; padding: 8px; border: 1px solid rgba(212,175,55,0.2); border-radius: 4px; cursor: pointer; transition: all 0.2s;
        &:hover { background: rgba(212,175,55,0.1); }
        &.other-month { opacity: 0.4; }
        &.today { border-color: #D4AF37; background: rgba(212,175,55,0.15);
          .day-number { color: #D4AF37; font-weight: 700; }
        }
        .day-number { font-size: 14px; font-weight: 500; }
        .schedule-count { margin-top: 8px; font-size: 12px; color: #D4AF37; background: rgba(212,175,55,0.2); padding: 2px 6px; border-radius: 4px; display: inline-block; }
      }
    }
  }
}
</style>
