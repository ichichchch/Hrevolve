<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { Search, Download } from '@element-plus/icons-vue';
import { taxApi } from '@/api';
import type { TaxRecord } from '@/types';

const { t } = useI18n();
const loading = ref(false);
const records = ref<TaxRecord[]>([]);
const searchKeyword = ref('');
const yearFilter = ref(new Date().getFullYear());
const monthFilter = ref<number | ''>('');
const pagination = ref({ page: 1, pageSize: 20, total: 0 });

// 年份选项
const yearOptions = computed(() => {
  const currentYear = new Date().getFullYear();
  return Array.from({ length: 5 }, (_, i) => currentYear - i);
});

// 月份选项（响应式）
const monthOptions = computed(() => 
  Array.from({ length: 12 }, (_, i) => ({ value: i + 1, label: t('taxExtra.monthLabel', { month: i + 1 }) }))
);

const fetchData = async () => {
  loading.value = true;
  try {
    const res = await taxApi.getTaxRecords({
      page: pagination.value.page,
      pageSize: pagination.value.pageSize,
    });
    records.value = res.data.items || res.data;
    pagination.value.total = res.data.total || records.value.length;
  } catch { /* ignore */ } finally { loading.value = false; }
};

// 导出
const handleExport = async () => {
  try {
    await taxApi.exportTaxRecords({ year: yearFilter.value, month: monthFilter.value || undefined });
  } catch { /* ignore */ }
};

// 合计方法
const getSummary = ({ columns, data }: { columns: any[]; data: any[] }) => {
  const sums: string[] = [];
  columns.forEach((column: any, index: number) => {
    if (index === 0) { sums[index] = t('tax.total'); return; }
    const values = data.map((item: any) => Number(item[column.property]));
    if (['grossIncome', 'deductions', 'taxableIncome', 'taxAmount'].includes(column.property)) {
      const sum = values.reduce((prev: number, curr: number) => prev + (curr || 0), 0);
      sums[index] = `¥${sum.toLocaleString()}`;
    } else {
      sums[index] = '';
    }
  });
  return sums;
};

onMounted(() => fetchData());
</script>

<template>
  <div class="tax-records-view">
    <el-card>
      <template #header>
        <div class="card-header">
          <span class="card-title">{{ t('tax.taxRecords') }}</span>
          <div class="header-actions">
            <el-select v-model="yearFilter" style="width: 100px" @change="fetchData">
              <el-option v-for="y in yearOptions" :key="y" :label="t('payroll.yearLabel', { year: y })" :value="y" />
            </el-select>
            <el-select v-model="monthFilter" :placeholder="t('tax.allMonths')" clearable style="width: 100px" @change="fetchData">
              <el-option v-for="m in monthOptions" :key="m.value" :label="m.label" :value="m.value" />
            </el-select>
            <el-input v-model="searchKeyword" :placeholder="t('tax.searchEmployee')" :prefix-icon="Search" clearable style="width: 160px" />
            <el-button :icon="Download" @click="handleExport">{{ t('tax.export') }}</el-button>
          </div>
        </div>
      </template>
      
      <el-table v-loading="loading" :data="records" stripe show-summary :summary-method="getSummary">
        <el-table-column prop="employeeName" :label="t('tax.employee')" width="120" />
        <el-table-column prop="period" :label="t('tax.taxPeriod')" width="100">
          <template #default="{ row }">{{ row.year }}-{{ String(row.month).padStart(2, '0') }}</template>
        </el-table-column>
        <el-table-column prop="grossIncome" :label="t('tax.grossIncome')" width="120">
          <template #default="{ row }">¥{{ row.grossIncome?.toLocaleString() }}</template>
        </el-table-column>
        <el-table-column prop="deductions" :label="t('tax.deductionsCol')" width="120">
          <template #default="{ row }">¥{{ row.deductions?.toLocaleString() }}</template>
        </el-table-column>
        <el-table-column prop="taxableIncome" :label="t('tax.taxableIncome')" width="120">
          <template #default="{ row }">¥{{ row.taxableIncome?.toLocaleString() }}</template>
        </el-table-column>
        <el-table-column prop="taxAmount" :label="t('tax.taxAmount')" width="120">
          <template #default="{ row }">¥{{ row.taxAmount?.toLocaleString() }}</template>
        </el-table-column>
        <el-table-column prop="status" :label="t('common.status')" width="100">
          <template #default="{ row }">
            <el-tag :type="row.status === 'filed' ? 'success' : row.status === 'pending' ? 'warning' : 'info'" size="small">
              {{ row.status === 'filed' ? t('tax.statusFiled') : row.status === 'pending' ? t('tax.statusPending') : t('tax.statusDraft') }}
            </el-tag>
          </template>
        </el-table-column>
      </el-table>
      
      <div class="pagination-wrapper">
        <el-pagination v-model:current-page="pagination.page" :page-size="pagination.pageSize" :total="pagination.total" layout="total, prev, pager, next" @current-change="fetchData" />
      </div>
    </el-card>
  </div>
</template>

<style scoped lang="scss">
.tax-records-view {
  .card-header { display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; gap: 12px;
    .card-title { font-size: 16px; font-weight: 600; }
    .header-actions { display: flex; gap: 12px; flex-wrap: wrap; }
  }
  .pagination-wrapper { margin-top: 16px; display: flex; justify-content: flex-end; }
}
</style>
