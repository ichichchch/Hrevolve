<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Plus, Edit, Delete, Search } from '@element-plus/icons-vue';
import { taxApi } from '@/api';
import type { TaxProfile } from '@/types';

const loading = ref(false);
const profiles = ref<TaxProfile[]>([]);
const dialogVisible = ref(false);
const dialogTitle = ref('');
const form = ref<Partial<TaxProfile>>({});
const saving = ref(false);
const searchKeyword = ref('');
const pagination = ref({ page: 1, pageSize: 20, total: 0 });

const fetchData = async () => {
  loading.value = true;
  try {
    const res = await taxApi.getTaxProfiles({ page: pagination.value.page, pageSize: pagination.value.pageSize });
    profiles.value = res.data.items || res.data;
    pagination.value.total = res.data.total || profiles.value.length;
  } catch { /* ignore */ } finally { loading.value = false; }
};

const handleAdd = () => {
  form.value = { isActive: true };
  dialogTitle.value = '新增税务档案';
  dialogVisible.value = true;
};

const handleEdit = (item: TaxProfile) => {
  form.value = { ...item };
  dialogTitle.value = '编辑税务档案';
  dialogVisible.value = true;
};

const handleDelete = async (item: TaxProfile) => {
  await ElMessageBox.confirm(`确定删除该税务档案吗？`, '提示', { type: 'warning' });
  try {
    await taxApi.deleteTaxProfile(item.id);
    ElMessage.success('删除成功');
    fetchData();
  } catch { /* ignore */ }
};

const handleSave = async () => {
  if (!form.value.employeeId || !form.value.taxNumber) {
    ElMessage.warning('请填写必填项');
    return;
  }
  saving.value = true;
  try {
    if (form.value.id) {
      await taxApi.updateTaxProfile(form.value.id, form.value);
    } else {
      await taxApi.createTaxProfile(form.value);
    }
    ElMessage.success('保存成功');
    dialogVisible.value = false;
    fetchData();
  } catch { /* ignore */ } finally { saving.value = false; }
};

onMounted(() => fetchData());
</script>

<template>
  <div class="tax-profiles-view">
    <el-card>
      <template #header>
        <div class="card-header">
          <span class="card-title">税务档案管理</span>
          <div class="header-actions">
            <el-input v-model="searchKeyword" placeholder="搜索" :prefix-icon="Search" clearable style="width: 180px" />
            <el-button type="primary" :icon="Plus" @click="handleAdd">新增档案</el-button>
          </div>
        </div>
      </template>
      
      <el-table v-loading="loading" :data="profiles" stripe>
        <el-table-column prop="employeeName" label="员工" width="120" />
        <el-table-column prop="taxNumber" label="税号" width="180" />
        <el-table-column prop="taxType" label="纳税类型" width="120">
          <template #default="{ row }">
            <el-tag size="small">{{ row.taxType === 'resident' ? '居民' : '非居民' }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="deductions" label="专项扣除" min-width="150">
          <template #default="{ row }">¥{{ row.deductions?.toLocaleString() || 0 }}</template>
        </el-table-column>
        <el-table-column prop="isActive" label="状态" width="80">
          <template #default="{ row }">
            <el-tag :type="row.isActive ? 'success' : 'danger'" size="small">{{ row.isActive ? '有效' : '无效' }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="120" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" size="small" @click="handleEdit(row)"><el-icon><Edit /></el-icon></el-button>
            <el-button link type="danger" size="small" @click="handleDelete(row)"><el-icon><Delete /></el-icon></el-button>
          </template>
        </el-table-column>
      </el-table>
      
      <div class="pagination-wrapper">
        <el-pagination v-model:current-page="pagination.page" :page-size="pagination.pageSize" :total="pagination.total" layout="total, prev, pager, next" @current-change="fetchData" />
      </div>
    </el-card>
    
    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="550px">
      <el-form :model="form" label-width="100px">
        <el-form-item label="员工ID" required><el-input v-model="form.employeeId" placeholder="请输入员工ID" /></el-form-item>
        <el-form-item label="税号" required><el-input v-model="form.taxNumber" placeholder="请输入税号" /></el-form-item>
        <el-form-item label="纳税类型">
          <el-select v-model="form.taxType" style="width: 100%">
            <el-option label="居民" value="resident" />
            <el-option label="非居民" value="non-resident" />
          </el-select>
        </el-form-item>
        <el-form-item label="专项扣除"><el-input-number v-model="form.deductions" :min="0" :precision="2" style="width: 100%" /></el-form-item>
        <el-form-item label="状态"><el-switch v-model="form.isActive" active-text="有效" inactive-text="无效" /></el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="saving" @click="handleSave">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<style scoped lang="scss">
.tax-profiles-view {
  .card-header { display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; gap: 12px;
    .card-title { font-size: 16px; font-weight: 600; }
    .header-actions { display: flex; gap: 12px; }
  }
  .pagination-wrapper { margin-top: 16px; display: flex; justify-content: flex-end; }
}
</style>
