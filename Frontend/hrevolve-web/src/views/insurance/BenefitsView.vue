<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Plus, Edit, Delete } from '@element-plus/icons-vue';
import { insuranceApi } from '@/api';
import type { Benefit } from '@/types';

const loading = ref(false);
const benefits = ref<Benefit[]>([]);
const dialogVisible = ref(false);
const dialogTitle = ref('');
const form = ref<Partial<Benefit>>({});
const saving = ref(false);

// 福利类型
const benefitTypes = [
  { value: 'meal', label: '餐饮补贴' },
  { value: 'transport', label: '交通补贴' },
  { value: 'housing', label: '住房补贴' },
  { value: 'communication', label: '通讯补贴' },
  { value: 'education', label: '教育培训' },
  { value: 'health', label: '健康体检' },
  { value: 'other', label: '其他福利' },
];

const fetchData = async () => {
  loading.value = true;
  try {
    const res = await insuranceApi.getBenefits();
    benefits.value = res.data;
  } catch { /* ignore */ } finally { loading.value = false; }
};

const handleAdd = () => {
  form.value = { type: 'meal', isActive: true };
  dialogTitle.value = '新增福利项目';
  dialogVisible.value = true;
};

const handleEdit = (item: Benefit) => {
  form.value = { ...item };
  dialogTitle.value = '编辑福利项目';
  dialogVisible.value = true;
};

const handleDelete = async (item: Benefit) => {
  await ElMessageBox.confirm(`确定删除"${item.name}"吗？`, '提示', { type: 'warning' });
  try {
    await insuranceApi.deleteBenefit(item.id);
    ElMessage.success('删除成功');
    fetchData();
  } catch { /* ignore */ }
};

const handleSave = async () => {
  if (!form.value.name || !form.value.type) {
    ElMessage.warning('请填写必填项');
    return;
  }
  saving.value = true;
  try {
    if (form.value.id) {
      await insuranceApi.updateBenefit(form.value.id, form.value);
    } else {
      await insuranceApi.createBenefit(form.value);
    }
    ElMessage.success('保存成功');
    dialogVisible.value = false;
    fetchData();
  } catch { /* ignore */ } finally { saving.value = false; }
};

const getTypeName = (type: string) => benefitTypes.find(t => t.value === type)?.label || type;

onMounted(() => fetchData());
</script>

<template>
  <div class="benefits-view">
    <el-card>
      <template #header>
        <div class="card-header">
          <span class="card-title">福利项目管理</span>
          <el-button type="primary" :icon="Plus" @click="handleAdd">新增福利</el-button>
        </div>
      </template>
      
      <el-table v-loading="loading" :data="benefits" stripe>
        <el-table-column prop="name" label="名称" min-width="150" />
        <el-table-column prop="type" label="类型" width="120">
          <template #default="{ row }"><el-tag size="small">{{ getTypeName(row.type) }}</el-tag></template>
        </el-table-column>
        <el-table-column prop="amount" label="金额" width="120">
          <template #default="{ row }">¥{{ row.amount }}/月</template>
        </el-table-column>
        <el-table-column prop="isActive" label="状态" width="80">
          <template #default="{ row }">
            <el-tag :type="row.isActive ? 'success' : 'danger'" size="small">{{ row.isActive ? '启用' : '禁用' }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="description" label="描述" min-width="200" show-overflow-tooltip />
        <el-table-column label="操作" width="120" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" size="small" @click="handleEdit(row)"><el-icon><Edit /></el-icon></el-button>
            <el-button link type="danger" size="small" @click="handleDelete(row)"><el-icon><Delete /></el-icon></el-button>
          </template>
        </el-table-column>
      </el-table>
      
      <el-empty v-if="!loading && benefits.length === 0" description="暂无福利项目" />
    </el-card>
    
    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="500px">
      <el-form :model="form" label-width="80px">
        <el-form-item label="名称" required><el-input v-model="form.name" placeholder="请输入福利名称" /></el-form-item>
        <el-form-item label="类型" required>
          <el-select v-model="form.type" style="width: 100%">
            <el-option v-for="t in benefitTypes" :key="t.value" :label="t.label" :value="t.value" />
          </el-select>
        </el-form-item>
        <el-form-item label="金额"><el-input-number v-model="form.amount" :min="0" :precision="2" style="width: 100%" /><span style="margin-left: 8px">/月</span></el-form-item>
        <el-form-item label="描述"><el-input v-model="form.description" type="textarea" :rows="2" /></el-form-item>
        <el-form-item label="状态"><el-switch v-model="form.isActive" active-text="启用" inactive-text="禁用" /></el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="saving" @click="handleSave">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<style scoped lang="scss">
.benefits-view {
  .card-header { display: flex; justify-content: space-between; align-items: center;
    .card-title { font-size: 16px; font-weight: 600; }
  }
}
</style>
