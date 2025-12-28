<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { leaveApi } from '@/api';
import type { LeaveType } from '@/types';

const { t } = useI18n();
const types = ref<LeaveType[]>([]);
const loading = ref(false);

const fetchTypes = async () => {
  loading.value = true;
  try { 
    const res = await leaveApi.getTypes(); 
    types.value = res.data; 
  } catch { /* ignore */ } finally { loading.value = false; }
};

onMounted(() => fetchTypes());
</script>

<template>
  <div class="types-view">
    <el-card>
      <template #header><span>{{ t('leaveAdmin.typeManagement') }}</span></template>
      <el-table :data="types" v-loading="loading" stripe>
        <el-table-column prop="code" :label="t('leaveAdmin.typeCode')" width="100" />
        <el-table-column prop="name" :label="t('leaveAdmin.typeName')" width="150" />
        <el-table-column prop="defaultDays" :label="t('leaveAdmin.defaultDays')" width="100" />
        <el-table-column prop="maxCarryOver" :label="t('leaveAdmin.maxCarryOver')" width="100" />
        <el-table-column prop="isPaid" :label="t('leaveAdmin.isPaid')" width="80">
          <template #default="{ row }">
            <el-tag :type="row.isPaid ? 'success' : 'info'" size="small">{{ row.isPaid ? t('common.yes') : t('common.no') }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="description" :label="t('settings.description')" />
      </el-table>
    </el-card>
  </div>
</template>
