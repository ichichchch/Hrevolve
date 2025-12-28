<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { organizationApi } from '@/api';
import type { Position } from '@/types';

const { t } = useI18n();
const positions = ref<Position[]>([]);
const loading = ref(false);

const fetchPositions = async () => {
  loading.value = true;
  try {
    const res = await organizationApi.getPositions();
    positions.value = res.data;
  } catch { /* ignore */ } finally { loading.value = false; }
};

onMounted(() => fetchPositions());
</script>

<template>
  <div class="positions-view">
    <el-card>
      <template #header><span>{{ t('organization.positionManagement') }}</span></template>
      <el-table :data="positions" v-loading="loading" stripe>
        <el-table-column prop="code" :label="t('organization.positionCode')" width="120" />
        <el-table-column prop="name" :label="t('organization.positionName')" width="200" />
        <el-table-column prop="level" :label="t('organization.positionLevel')" width="100" />
        <el-table-column prop="description" :label="t('organization.positionDescription')" />
      </el-table>
    </el-card>
  </div>
</template>
