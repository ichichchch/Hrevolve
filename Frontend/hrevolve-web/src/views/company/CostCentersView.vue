<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Plus, Edit, Delete } from '@element-plus/icons-vue';
import { companyApi } from '@/api';
import type { CostCenter } from '@/types';

const { t } = useI18n();

const loading = ref(false);
const treeData = ref<CostCenter[]>([]);
const dialogVisible = ref(false);
const dialogTitle = ref('');
const form = ref<Partial<CostCenter>>({});
const saving = ref(false);

const defaultProps = { children: 'children', label: 'name' };

const fetchData = async () => {
  loading.value = true;
  try {
    const res = await companyApi.getCostCenterTree();
    treeData.value = res.data;
  } catch { /* ignore */ } finally { loading.value = false; }
};

const handleAdd = (parent?: CostCenter) => {
  form.value = { parentId: parent?.id, isActive: true };
  dialogTitle.value = parent ? t('costCenters.addChild', { name: parent.name }) : t('costCenters.management');
  dialogVisible.value = true;
};

const handleEdit = (data: CostCenter) => {
  form.value = { ...data };
  dialogTitle.value = t('costCenters.edit');
  dialogVisible.value = true;
};

const handleDelete = async (data: CostCenter) => {
  await ElMessageBox.confirm(t('costCenters.confirmDelete', { name: data.name }), t('assistantExtra.tip'), { type: 'warning' });
  try {
    await companyApi.deleteCostCenter(data.id);
    ElMessage.success(t('common.success'));
    fetchData();
  } catch { /* ignore */ }
};

const handleSave = async () => {
  saving.value = true;
  try {
    if (form.value.id) {
      await companyApi.updateCostCenter(form.value.id, form.value);
    } else {
      await companyApi.createCostCenter(form.value);
    }
    ElMessage.success(t('common.success'));
    dialogVisible.value = false;
    fetchData();
  } catch { /* ignore */ } finally { saving.value = false; }
};

onMounted(() => fetchData());
</script>

<template>
  <div class="cost-centers-view">
    <el-card>
      <template #header>
        <div class="card-header">
          <span class="card-title">{{ t('costCenters.management') }}</span>
          <el-button type="primary" :icon="Plus" @click="handleAdd()">{{ t('costCenters.addRoot') }}</el-button>
        </div>
      </template>
      
      <el-tree
        v-loading="loading"
        :data="treeData"
        :props="defaultProps"
        default-expand-all
        node-key="id"
      >
        <template #default="{ node, data }">
          <div class="tree-node">
            <div class="node-content">
              <span class="node-name">{{ node.label }}</span>
              <el-tag v-if="data.code" size="small" type="info">{{ data.code }}</el-tag>
              <el-tag v-if="!data.isActive" size="small" type="danger">{{ t('costCenters.disabled') }}</el-tag>
            </div>
            <div class="node-actions">
              <el-button link type="primary" size="small" @click.stop="handleAdd(data)">
                <el-icon><Plus /></el-icon>
              </el-button>
              <el-button link type="primary" size="small" @click.stop="handleEdit(data)">
                <el-icon><Edit /></el-icon>
              </el-button>
              <el-button link type="danger" size="small" @click.stop="handleDelete(data)">
                <el-icon><Delete /></el-icon>
              </el-button>
            </div>
          </div>
        </template>
      </el-tree>
      
      <el-empty v-if="!loading && treeData.length === 0" :description="t('costCenters.noData')" />
    </el-card>
    
    <!-- 编辑对话框 -->
    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="500px">
      <el-form :model="form" label-width="100px">
        <el-form-item :label="t('costCenters.code')" required>
          <el-input v-model="form.code" :placeholder="t('costCenters.codePlaceholder')" />
        </el-form-item>
        <el-form-item :label="t('costCenters.name')" required>
          <el-input v-model="form.name" :placeholder="t('costCenters.namePlaceholder')" />
        </el-form-item>
        <el-form-item :label="t('costCenters.budget')">
          <el-input-number v-model="form.budget" :min="0" :precision="2" style="width: 100%" />
        </el-form-item>
        <el-form-item :label="t('costCenters.description')">
          <el-input v-model="form.description" type="textarea" :rows="2" />
        </el-form-item>
        <el-form-item :label="t('common.status')">
          <el-switch v-model="form.isActive" :active-text="t('settings.enabled')" :inactive-text="t('settings.disabled')" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">{{ t('common.cancel') }}</el-button>
        <el-button type="primary" :loading="saving" @click="handleSave">{{ t('common.save') }}</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<style scoped lang="scss">
.cost-centers-view {
  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    
    .card-title { font-size: 16px; font-weight: 600; }
  }
  
  .tree-node {
    flex: 1;
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding-right: 8px;
    
    .node-content {
      display: flex;
      align-items: center;
      gap: 8px;
    }
    
    .node-actions {
      opacity: 0;
      transition: opacity 0.2s;
    }
    
    &:hover .node-actions { opacity: 1; }
  }
}
</style>
