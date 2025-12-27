<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Plus, Edit, Delete } from '@element-plus/icons-vue';
import { companyApi } from '@/api';
import type { CostCenter } from '@/types';

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
  dialogTitle.value = parent ? `新增子成本中心 - ${parent.name}` : '新增成本中心';
  dialogVisible.value = true;
};

const handleEdit = (data: CostCenter) => {
  form.value = { ...data };
  dialogTitle.value = '编辑成本中心';
  dialogVisible.value = true;
};

const handleDelete = async (data: CostCenter) => {
  await ElMessageBox.confirm(`确定删除成本中心"${data.name}"吗？`, '提示', { type: 'warning' });
  try {
    await companyApi.deleteCostCenter(data.id);
    ElMessage.success('删除成功');
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
    ElMessage.success('保存成功');
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
          <span class="card-title">成本中心管理</span>
          <el-button type="primary" :icon="Plus" @click="handleAdd()">新增</el-button>
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
              <el-tag v-if="!data.isActive" size="small" type="danger">已禁用</el-tag>
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
      
      <el-empty v-if="!loading && treeData.length === 0" description="暂无成本中心数据" />
    </el-card>
    
    <!-- 编辑对话框 -->
    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="500px">
      <el-form :model="form" label-width="100px">
        <el-form-item label="编码" required>
          <el-input v-model="form.code" placeholder="请输入编码" />
        </el-form-item>
        <el-form-item label="名称" required>
          <el-input v-model="form.name" placeholder="请输入名称" />
        </el-form-item>
        <el-form-item label="预算">
          <el-input-number v-model="form.budget" :min="0" :precision="2" style="width: 100%" />
        </el-form-item>
        <el-form-item label="描述">
          <el-input v-model="form.description" type="textarea" :rows="2" />
        </el-form-item>
        <el-form-item label="状态">
          <el-switch v-model="form.isActive" active-text="启用" inactive-text="禁用" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="saving" @click="handleSave">保存</el-button>
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
