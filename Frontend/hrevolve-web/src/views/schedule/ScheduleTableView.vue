<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { ElMessage } from 'element-plus';
import { ArrowLeft, ArrowRight, Plus } from '@element-plus/icons-vue';
import { scheduleApi } from '@/api';
import type { Schedule, ShiftTemplate } from '@/types';

const { t } = useI18n();
const loading = ref(false);
const currentWeekStart = ref(getWeekStart(new Date()));
const schedules = ref<Schedule[]>([]);
const employees = ref<any[]>([]);
const shiftTemplates = ref<ShiftTemplate[]>([]);
const dialogVisible = ref(false);
const editingCell = ref<{ employeeId: string; date: string } | null>(null);
const selectedShift = ref('');

function getWeekStart(date: Date): Date {
  const d = new Date(date);
  const day = d.getDay();
  const diff = d.getDate() - day + (day === 0 ? -6 : 1);
  return new Date(d.setDate(diff));
}

const weekDates = computed(() => {
  const dates: Date[] = [];
  for (let i = 0; i < 7; i++) {
    const d = new Date(currentWeekStart.value);
    d.setDate(d.getDate() + i);
    dates.push(d);
  }
  return dates;
});

const formatDate = (date: Date) => date.toISOString().split('T')[0];

// 获取星期几的翻译（包含前缀）
const getWeekdayLabel = (date: Date) => {
  const weekdayKeys = ['sun', 'mon', 'tue', 'wed', 'thu', 'fri', 'sat'];
  const prefix = t('weekdays.prefix');
  const weekday = t(`weekdays.${weekdayKeys[date.getDay()]}`);
  return `${prefix}${weekday}`;
};

const getSchedule = (employeeId: string, date: Date) => {
  const dateStr = formatDate(date);
  return schedules.value.find(s => s.employeeId === employeeId && s.date === dateStr);
};

const getShiftName = (shiftId: string) => shiftTemplates.value.find(s => s.id === shiftId)?.name || '-';

const changeWeek = (delta: number) => {
  const d = new Date(currentWeekStart.value);
  d.setDate(d.getDate() + delta * 7);
  currentWeekStart.value = d;
  fetchData();
};

const fetchData = async () => {
  loading.value = true;
  try {
    const startDate = formatDate(currentWeekStart.value);
    const endDate = formatDate(weekDates.value[6]);
    const [schedulesRes, employeesRes, shiftsRes] = await Promise.all([
      scheduleApi.getSchedules({ page: 1, pageSize: 100, startDate, endDate }),
      scheduleApi.getSchedulableEmployees(),
      scheduleApi.getShiftTemplates()
    ]);
    schedules.value = schedulesRes.data.items || schedulesRes.data;
    employees.value = employeesRes.data;
    shiftTemplates.value = shiftsRes.data;
  } catch { /* ignore */ } finally { loading.value = false; }
};

const handleCellClick = (employeeId: string, date: Date) => {
  editingCell.value = { employeeId, date: formatDate(date) };
  const existing = getSchedule(employeeId, date);
  selectedShift.value = existing?.shiftTemplateId || '';
  dialogVisible.value = true;
};

const handleSaveSchedule = async () => {
  if (!editingCell.value) return;
  try {
    await scheduleApi.assignSchedule({
      employeeId: editingCell.value.employeeId,
      date: editingCell.value.date,
      shiftTemplateId: selectedShift.value || null
    });
    ElMessage.success(t('schedule.scheduleUpdated'));
    dialogVisible.value = false;
    fetchData();
  } catch { /* ignore */ }
};

onMounted(() => fetchData());
</script>

<template>
  <div class="schedule-table-view">
    <el-card>
      <template #header>
        <div class="card-header">
          <span class="card-title">{{ t('schedule.scheduleTable') }}</span>
          <div class="week-nav">
            <el-button :icon="ArrowLeft" @click="changeWeek(-1)">{{ t('schedule.prevWeek') }}</el-button>
            <span class="week-label">{{ formatDate(currentWeekStart) }} ~ {{ formatDate(weekDates[6]) }}</span>
            <el-button @click="changeWeek(1)">{{ t('schedule.nextWeek') }}<el-icon class="el-icon--right"><ArrowRight /></el-icon></el-button>
          </div>
        </div>
      </template>
      <div class="schedule-grid" v-loading="loading">
        <table class="schedule-table">
          <thead>
            <tr>
              <th class="employee-col">{{ t('schedule.employee') }}</th>
              <th v-for="date in weekDates" :key="formatDate(date)" class="date-col">
                <div class="date-header">
                  <span class="weekday">{{ getWeekdayLabel(date) }}</span>
                  <span class="date">{{ date.getDate() }}</span>
                </div>
              </th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="emp in employees" :key="emp.id">
              <td class="employee-col">
                <div class="employee-info">
                  <el-avatar :size="32">{{ emp.name?.charAt(0) }}</el-avatar>
                  <span>{{ emp.name }}</span>
                </div>
              </td>
              <td v-for="date in weekDates" :key="formatDate(date)" class="schedule-cell" @click="handleCellClick(emp.id, date)">
                <div v-if="getSchedule(emp.id, date)" class="shift-tag">{{ getShiftName(getSchedule(emp.id, date)!.shiftTemplateId) }}</div>
                <el-icon v-else class="add-icon"><Plus /></el-icon>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      <el-empty v-if="!loading && employees.length === 0" :description="t('schedule.noSchedulableEmployees')" />
    </el-card>
    <el-dialog v-model="dialogVisible" :title="t('schedule.setSchedule')" width="400px">
      <el-form label-width="80px">
        <el-form-item :label="t('schedule.shift')">
          <el-select v-model="selectedShift" :placeholder="t('schedule.selectShift')" clearable style="width: 100%">
            <el-option v-for="s in shiftTemplates" :key="s.id" :label="s.name" :value="s.id">
              <span>{{ s.name }}</span>
              <span style="color: rgba(255,255,255,0.5); margin-left: 8px">{{ s.startTime }}-{{ s.endTime }}</span>
            </el-option>
          </el-select>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">{{ t('common.cancel') }}</el-button>
        <el-button type="primary" @click="handleSaveSchedule">{{ t('common.save') }}</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<style scoped lang="scss">
.schedule-table-view {
  .card-header { display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; gap: 12px;
    .card-title { font-size: 16px; font-weight: 600; }
    .week-nav { display: flex; align-items: center; gap: 12px;
      .week-label { font-size: 14px; font-weight: 500; min-width: 180px; text-align: center; }
    }
  }
  .schedule-grid { overflow-x: auto; }
  .schedule-table { width: 100%; border-collapse: collapse;
    th, td { border: 1px solid rgba(212,175,55,0.2); padding: 8px; text-align: center; }
    th { background: rgba(212,175,55,0.1); }
    .employee-col { width: 150px; text-align: left; }
    .date-col { min-width: 80px; }
    .date-header { .weekday { display: block; font-size: 12px; color: rgba(255,255,255,0.65); }
      .date { display: block; font-size: 16px; font-weight: 600; }
    }
    .employee-info { display: flex; align-items: center; gap: 8px; }
    .schedule-cell { cursor: pointer; height: 50px; transition: background 0.2s;
      &:hover { background: rgba(212,175,55,0.1); }
      .shift-tag { background: linear-gradient(135deg, #D4AF37, #B8860B); color: #0D0D0D; padding: 4px 8px; border-radius: 4px; font-size: 12px; font-weight: 500; }
      .add-icon { color: rgba(255,255,255,0.3); font-size: 20px; }
    }
  }
}
</style>
