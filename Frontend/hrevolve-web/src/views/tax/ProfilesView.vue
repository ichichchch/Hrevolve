<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Plus, Edit, Delete, Search } from '@element-plus/icons-vue';
import { taxApi } from '@/api';
import type { TaxProfile } from '@/types';

const { t } = useI18n();
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
  dialogTitle.value = t('tax.newProfile');
  dialogVisible.value = true;
};

const handleEdit = (item: TaxProfile) => {
  form.value = { ...item };
  dialogTitle.value = t('tax.editProfile');
  dialogVisible.value = true;
};

const handleDelete = async (item: TaxProfile) => {
  await ElMessageBox.confirm(t('tax.confirmDeleteProfile'), t('common.confirm'), { type: 'warning' });
  try {
    await taxApi.deleteTaxProfile(item.id);
    ElMessage.success(t('tax.deleteSuccess'));
    fetchData();
  } catch { /* ignore */ }
};

const handleSave = async () => {
  if (!form.value.employeeId || !form.value.taxNumber) {
    ElMessage.warning(t('tax.fillRequired'));
    return;
  }
  saving.value = true;
  try {
    if (form.value.id) {
      await taxApi.updateTaxProfile(form.value.id, form.value);
    } else {
      await taxApi.createTaxProfile(form.value);
    }
    ElMessage.success(t('common.success'));
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
          <span class="card-title">{{ t('tax.taxProfiles') }}</span>
          <div class="header-actions">
            <el-input v-model="searchKeyword" :placeholder="t('common.search')" :prefix-icon="Search" clearable style="width: 180px" />
            <el-button type="primary" :icon="Plus" @click="handleAdd">{{ t('tax.addProfile') }}</el-button>
          </div>
        </div>
      </template>
      
      <el-table v-loading="loading" :data="profiles" stripe>
        <el-table-column prop="employeeName" :label="t('tax.employee')" width="120" />
        <el-table-column prop="taxNumber" :label="t('tax.taxNumber')" width="180" />
        <el-table-column prop="taxType" :label="t('tax.taxType')" width="120">
          <template #default="{ row }">
            <el-tag size="small">{{ row.taxType === 'resident' ? t('tax.resident') : t('tax.nonResident') }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="deductions" :label="t('tax.specialDeductionsCol')" min-width="150">
          <template #default="{ row }">¥{{ row.deductions?.toLocaleString() || 0 }}</template>
        </el-table-column>
        <el-table-column prop="isActive" :label="t('common.status')" width="80">
          <template #default="{ row }">
            <el-tag :type="row.isActive ? 'success' : 'danger'" size="small">{{ row.isActive ? t('tax.valid') : t('tax.invalid') }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column :label="t('common.actions')" width="120" fixed="right">
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
        <el-form-item :label="t('employee.employeeNo')" required><el-input v-model="form.employeeId" :placeholder="t('tax.enterEmployeeId')" /></el-form-item>
        <el-form-item :label="t('tax.taxNumber')" required><el-input v-model="form.taxNumber" :placeholder="t('tax.enterTaxNumber')" /></el-form-item>
        <el-form-item :label="t('tax.taxType')">
          <el-select v-model="form.taxType" style="width: 100%">
            <el-option :label="t('tax.resident')" value="resident" />
            <el-option :label="t('tax.nonResident')" value="non-resident" />
          </el-select>
        </el-form-item>
        <el-form-item :label="t('tax.specialDeductionsCol')"><el-input-number v-model="form.deductions" :min="0" :precision="2" style="width: 100%" /></el-form-item>
        <el-form-item :label="t('common.status')"><el-switch v-model="form.isActive" :active-text="t('tax.valid')" :inactive-text="t('tax.invalid')" /></el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">{{ t('common.cancel') }}</el-button>
        <el-button type="primary" :loading="saving" @click="handleSave">{{ t('common.save') }}</el-button>
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
