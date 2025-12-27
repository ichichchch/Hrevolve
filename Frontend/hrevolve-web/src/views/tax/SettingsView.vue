<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import { taxApi } from '@/api';

const loading = ref(false);
const saving = ref(false);
const settings = ref({
  taxYear: new Date().getFullYear(),
  basicDeduction: 5000,
  socialInsuranceRate: 0.105,
  housingFundRate: 0.12,
  childEducation: 1000,
  continuingEducation: 400,
  housingLoanInterest: 1000,
  housingRent: 1500,
  elderlySupport: 2000,
  infantCare: 1000,
});

// 税率表
const taxBrackets = ref([
  { min: 0, max: 36000, rate: 0.03, deduction: 0 },
  { min: 36000, max: 144000, rate: 0.10, deduction: 2520 },
  { min: 144000, max: 300000, rate: 0.20, deduction: 16920 },
  { min: 300000, max: 420000, rate: 0.25, deduction: 31920 },
  { min: 420000, max: 660000, rate: 0.30, deduction: 52920 },
  { min: 660000, max: 960000, rate: 0.35, deduction: 85920 },
  { min: 960000, max: Infinity, rate: 0.45, deduction: 181920 },
]);

const fetchData = async () => {
  loading.value = true;
  try {
    const res = await taxApi.getTaxSettings();
    if (res.data) {
      settings.value = { ...settings.value, ...res.data };
    }
  } catch { /* ignore */ } finally { loading.value = false; }
};

const handleSave = async () => {
  saving.value = true;
  try {
    await taxApi.updateTaxSettings(settings.value);
    ElMessage.success('保存成功');
  } catch { /* ignore */ } finally { saving.value = false; }
};

onMounted(() => fetchData());
</script>

<template>
  <div class="tax-settings-view">
    <el-row :gutter="16">
      <el-col :xs="24" :lg="12">
        <el-card>
          <template #header><span class="card-title">基本设置</span></template>
          <el-form :model="settings" label-width="140px" v-loading="loading">
            <el-form-item label="税务年度">
              <el-input-number v-model="settings.taxYear" :min="2020" :max="2030" />
            </el-form-item>
            <el-form-item label="基本减除费用">
              <el-input-number v-model="settings.basicDeduction" :min="0" :step="100" />
              <span style="margin-left: 8px">/月</span>
            </el-form-item>
            <el-form-item label="社保缴纳比例">
              <el-input-number v-model="settings.socialInsuranceRate" :min="0" :max="1" :precision="3" :step="0.01" />
            </el-form-item>
            <el-form-item label="公积金缴纳比例">
              <el-input-number v-model="settings.housingFundRate" :min="0" :max="0.24" :precision="2" :step="0.01" />
            </el-form-item>
          </el-form>
        </el-card>
        
        <el-card style="margin-top: 16px">
          <template #header><span class="card-title">专项附加扣除标准</span></template>
          <el-form :model="settings" label-width="140px">
            <el-form-item label="子女教育"><el-input-number v-model="settings.childEducation" :min="0" :step="100" /><span style="margin-left: 8px">/月/子女</span></el-form-item>
            <el-form-item label="继续教育"><el-input-number v-model="settings.continuingEducation" :min="0" :step="100" /><span style="margin-left: 8px">/月</span></el-form-item>
            <el-form-item label="住房贷款利息"><el-input-number v-model="settings.housingLoanInterest" :min="0" :step="100" /><span style="margin-left: 8px">/月</span></el-form-item>
            <el-form-item label="住房租金"><el-input-number v-model="settings.housingRent" :min="0" :step="100" /><span style="margin-left: 8px">/月</span></el-form-item>
            <el-form-item label="赡养老人"><el-input-number v-model="settings.elderlySupport" :min="0" :step="100" /><span style="margin-left: 8px">/月</span></el-form-item>
            <el-form-item label="婴幼儿照护"><el-input-number v-model="settings.infantCare" :min="0" :step="100" /><span style="margin-left: 8px">/月/子女</span></el-form-item>
            <el-form-item>
              <el-button type="primary" :loading="saving" @click="handleSave">保存设置</el-button>
            </el-form-item>
          </el-form>
        </el-card>
      </el-col>
      
      <el-col :xs="24" :lg="12">
        <el-card>
          <template #header><span class="card-title">个人所得税税率表（综合所得）</span></template>
          <el-table :data="taxBrackets" stripe>
            <el-table-column label="级数" width="60" type="index" :index="(i: number) => i + 1" />
            <el-table-column label="全年应纳税所得额" min-width="180">
              <template #default="{ row }">
                {{ row.max === Infinity ? `超过${row.min.toLocaleString()}元` : `${row.min.toLocaleString()} - ${row.max.toLocaleString()}元` }}
              </template>
            </el-table-column>
            <el-table-column prop="rate" label="税率" width="80">
              <template #default="{ row }">{{ (row.rate * 100).toFixed(0) }}%</template>
            </el-table-column>
            <el-table-column prop="deduction" label="速算扣除数" width="100">
              <template #default="{ row }">{{ row.deduction.toLocaleString() }}</template>
            </el-table-column>
          </el-table>
          <div class="tax-note">
            <p>注：本表所称全年应纳税所得额是指依照税法规定，居民个人取得综合所得以每一纳税年度收入额减除费用六万元以及专项扣除、专项附加扣除和依法确定的其他扣除后的余额。</p>
          </div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<style scoped lang="scss">
.tax-settings-view {
  .card-title { font-size: 16px; font-weight: 600; }
  .tax-note { margin-top: 16px; padding: 12px; background: rgba(212,175,55,0.1); border-radius: 4px;
    p { font-size: 12px; color: rgba(255,255,255,0.65); margin: 0; line-height: 1.6; }
  }
}
</style>
