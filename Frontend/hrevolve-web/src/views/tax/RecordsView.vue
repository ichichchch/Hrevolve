<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { Search, Download } from '@element-plus/icons-vue';
import { taxApi } from '@/api';
import type { TaxRecord } from '@/types';

const loading = ref(false);
const records = ref<TaxRecord[]>([]);
const searchKeyword = ref('');
const yearFilter = ref(new Date().getFullYear());
const monthFilter = ref('');
const pagination = ref({ page: 1, pageSize: 20, total: 0 });

// 年份选项
const yearOptions = computed(() => {
  const currentYear = new Date().getFullYear();
  return Array.from({ length: 5 }, (_, i) => currentYear - i);
});

// 月份选项
const monthOptions = Array.from({ length: 12 }, (_, i) => ({ value: i + 1, label: `${i + 1}月` }));

const fetchData = async () => {
  loading.value = true;
  try {
    const res = await taxApi.getTaxRecords({
      page: pagination.value.page,
      pageSize: pagination.value.pageSize,
      year: yearFilter.value,
      month: monthFilter.value || undefined
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

onMounted(() => fetchData());
</script>

<template>
  <div class="tax-records-view">
    <el-card>
      <template #header>
        <div class="card-header">
          <span class="card-title">报税记录</span>
          <div class="header-actions">
            <el-select v-model="yearFilter" style="width: 100px" @change="fetchData">
              <el-option v-for="y in yearOptions" :key="y" :label="`${y}年`" :value="y" />
            </el-select>
            <el-select v-model="monthFilter" placeholder="全部月份" clearable style="width: 100px" @change="fetchData">
              <el-option v-for="m in monthOptions" :key="m.value" :label="m.label" :value="m.value" />
            </el-select>
            <el-input v-model="searchKeyword" placeholder="搜索员工" :prefix-icon="Search" clearable style="width: 160px" />
            <el-button :icon="Download" @click="handleExport">导出</el-button>
          </div>
        </div>
      </template>
      
      <el-table v-loading="loading" :data="records" stripe show-summary :summary-method="getSummary">
        <el-table-column prop="employeeName" label="员工" width="120" />
        <el-table-column prop="period" label="税期" width="100">
          <template #default="{ row }">{{ row.year }}-{{ String(row.month).padStart(2, '0') }}</template>
        </el-table-column>
        <el-table-column prop="grossIncome" label="应税收入" width="120">
          <template #default="{ row }">¥{{ row.grossIncome?.toLocaleString() }}</template>
        </el-table-column>
        <el-table-column prop="deductions" label="扣除项" width="120">
          <template #default="{ row }">¥{{ row.deductions?.toLocaleString() }}</template>
        </el-table-column>
        <el-table-column prop="taxableIncome" label="应纳税所得" width="120">
          <template #default="{ row }">¥{{ row.taxableIncome?.toLocaleString() }}</template>
        </el-table-column>
        <el-table-column prop="taxAmount" label="应纳税额" width="120">
          <template #default="{ row }">¥{{ row.taxAmount?.toLocaleString() }}</template>
        </el-table-column>
        <el-table-column prop="status" label="状态" width="100">
          <template #default="{ row }">
            <el-tag :type="row.status === 'filed' ? 'success' : row.status === 'pending' ? 'warning' : 'info'" size="small">
              {{ row.status === 'filed' ? '已申报' : row.status === 'pending' ? '待申报' : '草稿' }}
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

<script lang="ts">
// 合计方法
function getSummary({ columns, data }: any) {
  const sums: string[] = [];
  columns.forEach((column: any, index: number) => {
    if (index === 0) { sums[index] = '合计'; return; }
    const values = data.map((item: any) => Number(item[column.property]));
    if (['grossIncome', 'deductions', 'taxableIncome', 'taxAmount'].includes(column.property)) {
      const sum = values.reduce((prev: number, curr: number) => prev + (curr || 0), 0);
      sums[index] = `¥${sum.toLocaleString()}`;
    } else {
      sums[index] = '';
    }
  });
  return sums;
}
</script>

<style scoped lang="scss">
.tax-records-view {
  .card-header { display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; gap: 12px;
    .card-title { font-size: 16px; font-weight: 600; }
    .header-actions { display: flex; gap: 12px; flex-wrap: wrap; }
  }
  .pagination-wrapper { margin-top: 16px; display: flex; justify-content: flex-end; }
}
</style>
