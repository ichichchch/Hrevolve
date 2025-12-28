<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { organizationApi } from '@/api';
import type { OrganizationUnit } from '@/types';

const { t } = useI18n();
const treeData = ref<OrganizationUnit[]>([]);
const loading = ref(false);
const defaultProps = { children: 'children', label: 'name' };

const fetchTree = async () => {
  loading.value = true;
  try {
    const res = await organizationApi.getTree();
    treeData.value = res.data;
  } catch { /* ignore */ } finally { loading.value = false; }
};

onMounted(() => fetchTree());
</script>

<template>
  <div class="org-structure">
    <el-card>
      <template #header><span>{{ t('orgAdmin.structure') }}</span></template>
      <el-tree v-loading="loading" :data="treeData" :props="defaultProps" default-expand-all node-key="id">
        <template #default="{ node, data }">
          <span class="tree-node">
            <span>{{ node.label }}</span>
            <span class="node-info">{{ t('orgAdmin.employeeCount', { count: data.employeeCount || 0 }) }}</span>
          </span>
        </template>
      </el-tree>
      <el-empty v-if="!loading && treeData.length === 0" :description="t('orgAdmin.noData')" />
    </el-card>
  </div>
</template>

<style scoped lang="scss">
.org-structure { .tree-node { display: flex; gap: 8px; .node-info { color: #999; font-size: 12px; } } }
</style>
