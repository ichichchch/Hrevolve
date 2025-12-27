<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import { companyApi } from '@/api';
import type { Tenant } from '@/types';

const loading = ref(false);
const saving = ref(false);
const form = ref<Partial<Tenant>>({});

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
    ElMessage.success('保存成功');
  } catch { /* ignore */ } finally { saving.value = false; }
};

const handleLogoUpload = async (file: File) => {
  try {
    const res = await companyApi.uploadLogo(file);
    form.value.logo = res.data.url;
    ElMessage.success('Logo上传成功');
  } catch { /* ignore */ }
  return false;
};

onMounted(() => fetchTenant());
</script>

<template>
  <div class="tenant-view">
    <el-card v-loading="loading">
      <template #header>
        <span class="card-title">公司基本信息</span>
      </template>
      
      <el-form :model="form" label-width="120px" style="max-width: 600px">
        <el-form-item label="公司Logo">
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
        
        <el-form-item label="公司编码">
          <el-input v-model="form.code" disabled />
        </el-form-item>
        
        <el-form-item label="公司名称" required>
          <el-input v-model="form.name" placeholder="请输入公司名称" />
        </el-form-item>
        
        <el-form-item label="所属行业">
          <el-select v-model="form.industry" placeholder="请选择行业" style="width: 100%">
            <el-option label="互联网/IT" value="IT" />
            <el-option label="金融" value="Finance" />
            <el-option label="制造业" value="Manufacturing" />
            <el-option label="零售" value="Retail" />
            <el-option label="教育" value="Education" />
            <el-option label="医疗" value="Healthcare" />
            <el-option label="其他" value="Other" />
          </el-select>
        </el-form-item>
        
        <el-form-item label="公司规模">
          <el-select v-model="form.scale" placeholder="请选择规模" style="width: 100%">
            <el-option label="1-50人" value="1-50" />
            <el-option label="51-200人" value="51-200" />
            <el-option label="201-500人" value="201-500" />
            <el-option label="501-1000人" value="501-1000" />
            <el-option label="1000人以上" value="1000+" />
          </el-select>
        </el-form-item>
        
        <el-form-item label="联系电话">
          <el-input v-model="form.phone" placeholder="请输入联系电话" />
        </el-form-item>
        
        <el-form-item label="联系邮箱">
          <el-input v-model="form.email" placeholder="请输入联系邮箱" />
        </el-form-item>
        
        <el-form-item label="公司网站">
          <el-input v-model="form.website" placeholder="请输入公司网站" />
        </el-form-item>
        
        <el-form-item label="公司地址">
          <el-input v-model="form.address" type="textarea" :rows="2" placeholder="请输入公司地址" />
        </el-form-item>
        
        <el-form-item>
          <el-button type="primary" :loading="saving" @click="handleSave">保存</el-button>
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
