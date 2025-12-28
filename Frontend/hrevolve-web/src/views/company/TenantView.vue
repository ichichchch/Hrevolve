<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { ElMessage } from 'element-plus';
import { companyApi } from '@/api';
import type { Tenant } from '@/types';

const { t } = useI18n();
const loading = ref(false);
const saving = ref(false);
const form = ref<Partial<Tenant>>({});

// 行业选项（响应式）
const industryOptions = computed(() => [
  { label: t('company.industryIT'), value: 'IT' },
  { label: t('company.industryFinance'), value: 'Finance' },
  { label: t('company.industryManufacturing'), value: 'Manufacturing' },
  { label: t('company.industryRetail'), value: 'Retail' },
  { label: t('company.industryEducation'), value: 'Education' },
  { label: t('company.industryHealthcare'), value: 'Healthcare' },
  { label: t('company.industryOther'), value: 'Other' },
]);

// 规模选项（响应式）
const scaleOptions = computed(() => [
  { label: t('company.scale1_50'), value: '1-50' },
  { label: t('company.scale51_200'), value: '51-200' },
  { label: t('company.scale201_500'), value: '201-500' },
  { label: t('company.scale501_1000'), value: '501-1000' },
  { label: t('company.scale1000plus'), value: '1000+' },
]);

const fetchTenant = async () => {
  loading.value = true;
  try {
    const res = await companyApi.getTenant();
    form.value = res.data;
  } catch { /* ignore */ } finally { loading.value = false; }
};

const handleSave = async () => {
  saving.value = true;
  try {
    await companyApi.updateTenant(form.value);
    ElMessage.success(t('company.saveSuccess'));
  } catch { /* ignore */ } finally { saving.value = false; }
};

const handleLogoUpload = async (file: File) => {
  try {
    const res = await companyApi.uploadLogo(file);
    form.value.logo = res.data.url;
    ElMessage.success(t('company.logoUploadSuccess'));
  } catch { /* ignore */ }
  return false;
};

onMounted(() => fetchTenant());
</script>

<template>
  <div class="tenant-view">
    <el-card v-loading="loading">
      <template #header>
        <span class="card-title">{{ t('company.tenantInfo') }}</span>
      </template>
      
      <el-form :model="form" label-width="120px" style="max-width: 600px">
        <el-form-item :label="t('company.companyLogo')">
          <el-upload
            class="logo-uploader"
            :show-file-list="false"
            :before-upload="handleLogoUpload"
            accept="image/*"
          >
            <img v-if="form.logo" :src="form.logo" class="logo-preview" />
            <el-icon v-else class="logo-placeholder"><Plus /></el-icon>
          </el-upload>
        </el-form-item>
        
        <el-form-item :label="t('company.companyCode')">
          <el-input v-model="form.code" disabled />
        </el-form-item>
        
        <el-form-item :label="t('company.companyName')" required>
          <el-input v-model="form.name" :placeholder="t('company.placeholder.companyName')" />
        </el-form-item>
        
        <el-form-item :label="t('company.industry')">
          <el-select v-model="form.industry" :placeholder="t('company.placeholder.industry')" style="width: 100%">
            <el-option v-for="opt in industryOptions" :key="opt.value" :label="opt.label" :value="opt.value" />
          </el-select>
        </el-form-item>
        
        <el-form-item :label="t('company.companyScale')">
          <el-select v-model="form.scale" :placeholder="t('company.placeholder.scale')" style="width: 100%">
            <el-option v-for="opt in scaleOptions" :key="opt.value" :label="opt.label" :value="opt.value" />
          </el-select>
        </el-form-item>
        
        <el-form-item :label="t('company.contactPhone')">
          <el-input v-model="form.phone" :placeholder="t('company.placeholder.phone')" />
        </el-form-item>
        
        <el-form-item :label="t('company.contactEmail')">
          <el-input v-model="form.email" :placeholder="t('company.placeholder.email')" />
        </el-form-item>
        
        <el-form-item :label="t('company.companyWebsite')">
          <el-input v-model="form.website" :placeholder="t('company.placeholder.website')" />
        </el-form-item>
        
        <el-form-item :label="t('company.companyAddress')">
          <el-input v-model="form.address" type="textarea" :rows="2" :placeholder="t('company.placeholder.address')" />
        </el-form-item>
        
        <el-form-item>
          <el-button type="primary" :loading="saving" @click="handleSave">{{ t('common.save') }}</el-button>
        </el-form-item>
      </el-form>
    </el-card>
  </div>
</template>

<script lang="ts">
import { Plus } from '@element-plus/icons-vue';
export default { components: { Plus } };
</script>

<style scoped lang="scss">
.tenant-view {
  .card-title {
    font-size: 16px;
    font-weight: 600;
  }
  
  .logo-uploader {
    :deep(.el-upload) {
      border: 1px dashed rgba(212, 175, 55, 0.3);
      border-radius: 8px;
      cursor: pointer;
      width: 120px;
      height: 120px;
      display: flex;
      align-items: center;
      justify-content: center;
      transition: all 0.3s;
      
      &:hover {
        border-color: #D4AF37;
      }
    }
  }
  
  .logo-preview {
    width: 120px;
    height: 120px;
    object-fit: contain;
  }
  
  .logo-placeholder {
    font-size: 32px;
    color: rgba(255, 255, 255, 0.45);
  }
}
</style>
