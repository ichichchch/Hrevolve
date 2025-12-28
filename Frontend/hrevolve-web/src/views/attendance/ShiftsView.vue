<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { attendanceApi } from '@/api';
import type { Shift } from '@/types';

const { t } = useI18n();
const shifts = ref<Shift[]>([]);
const loading = ref(false);

const fetchShifts = async () => {
  loading.value = true;
  try { 
    const res = await attendanceApi.getShifts(); 
    shifts.value = res.data; 
  } catch { /* ignore */ } finally { loading.value = false; }
};

onMounted(() => fetchShifts());
</script>

<template>
  <div class="shifts-view">
    <el-card>
      <template #header><span>{{ t('shifts.management') }}</span></template>
      <el-table :data="shifts" v-loading="loading" stripe>
        <el-table-column prop="code" :label="t('shifts.code')" width="120" />
        <el-table-column prop="name" :label="t('shifts.name')" width="150" />
        <el-table-column prop="startTime" :label="t('shifts.startTime')" width="120" />
        <el-table-column prop="endTime" :label="t('shifts.endTime')" width="120" />
        <el-table-column prop="breakMinutes" :label="t('shifts.breakMinutes')" width="120" />
        <el-table-column prop="isFlexible" :label="t('shifts.isFlexible')">
          <template #default="{ row }">
            <el-tag :type="row.isFlexible ? 'success' : 'info'" size="small">{{ row.isFlexible ? t('common.yes') : t('common.no') }}</el-tag>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>
