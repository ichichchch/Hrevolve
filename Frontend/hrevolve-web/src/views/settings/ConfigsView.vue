<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import { settingsApi } from '@/api';

const loading = ref(false);
const saving = ref(false);
const activeTab = ref('general');

// 系统配置
const generalConfig = ref({
  systemName: 'Hrevolve',
  companyName: '',
  logo: '',
  timezone: 'Asia/Shanghai',
  dateFormat: 'YYYY-MM-DD',
  timeFormat: 'HH:mm:ss',
  language: 'zh-CN',
  currency: 'CNY',
});

// 安全配置
const securityConfig = ref({
  passwordMinLength: 8,
  passwordRequireUppercase: true,
  passwordRequireLowercase: true,
  passwordRequireNumber: true,
  passwordRequireSpecial: false,
  sessionTimeout: 30,
  maxLoginAttempts: 5,
  lockoutDuration: 15,
  enableTwoFactor: false,
});

// 通知配置
const notificationConfig = ref({
  enableEmail: true,
  enableSms: false,
  enablePush: true,
  emailServer: '',
  emailPort: 587,
  emailUsername: '',
  smsProvider: '',
  smsApiKey: '',
});

const fetchData = async () => {
  loading.value = true;
  try {
    const res = await settingsApi.getSystemConfigs();
    if (res.data) {
      generalConfig.value = { ...generalConfig.value, ...res.data.general };
      securityConfig.value = { ...securityConfig.value, ...res.data.security };
      notificationConfig.value = { ...notificationConfig.value, ...res.data.notification };
    }
  } catch { /* ignore */ } finally { loading.value = false; }
};

const handleSave = async () => {
  saving.value = true;
  try {
    await settingsApi.updateSystemConfigs({
      general: generalConfig.value,
      security: securityConfig.value,
      notification: notificationConfig.value,
    });
    ElMessage.success('保存成功');
  } catch { /* ignore */ } finally { saving.value = false; }
};

onMounted(() => fetchData());
</script>

<template>
  <div class="configs-view">
    <el-card v-loading="loading">
      <el-tabs v-model="activeTab">
        <el-tab-pane label="基本设置" name="general">
          <el-form :model="generalConfig" label-width="120px" style="max-width: 600px">
            <el-form-item label="系统名称"><el-input v-model="generalConfig.systemName" /></el-form-item>
            <el-form-item label="公司名称"><el-input v-model="generalConfig.companyName" /></el-form-item>
            <el-form-item label="时区">
              <el-select v-model="generalConfig.timezone" style="width: 100%">
                <el-option label="Asia/Shanghai (UTC+8)" value="Asia/Shanghai" />
                <el-option label="Asia/Tokyo (UTC+9)" value="Asia/Tokyo" />
                <el-option label="America/New_York (UTC-5)" value="America/New_York" />
                <el-option label="Europe/London (UTC+0)" value="Europe/London" />
              </el-select>
            </el-form-item>
            <el-form-item label="日期格式">
              <el-select v-model="generalConfig.dateFormat" style="width: 100%">
                <el-option label="YYYY-MM-DD" value="YYYY-MM-DD" />
                <el-option label="DD/MM/YYYY" value="DD/MM/YYYY" />
                <el-option label="MM/DD/YYYY" value="MM/DD/YYYY" />
              </el-select>
            </el-form-item>
            <el-form-item label="默认语言">
              <el-select v-model="generalConfig.language" style="width: 100%">
                <el-option label="简体中文" value="zh-CN" />
                <el-option label="English" value="en-US" />
              </el-select>
            </el-form-item>
            <el-form-item label="货币">
              <el-select v-model="generalConfig.currency" style="width: 100%">
                <el-option label="人民币 (CNY)" value="CNY" />
                <el-option label="美元 (USD)" value="USD" />
                <el-option label="欧元 (EUR)" value="EUR" />
              </el-select>
            </el-form-item>
          </el-form>
        </el-tab-pane>
        
        <el-tab-pane label="安全设置" name="security">
          <el-form :model="securityConfig" label-width="140px" style="max-width: 600px">
            <el-form-item label="密码最小长度"><el-input-number v-model="securityConfig.passwordMinLength" :min="6" :max="20" /></el-form-item>
            <el-form-item label="要求大写字母"><el-switch v-model="securityConfig.passwordRequireUppercase" /></el-form-item>
            <el-form-item label="要求小写字母"><el-switch v-model="securityConfig.passwordRequireLowercase" /></el-form-item>
            <el-form-item label="要求数字"><el-switch v-model="securityConfig.passwordRequireNumber" /></el-form-item>
            <el-form-item label="要求特殊字符"><el-switch v-model="securityConfig.passwordRequireSpecial" /></el-form-item>
            <el-divider />
            <el-form-item label="会话超时"><el-input-number v-model="securityConfig.sessionTimeout" :min="5" :max="120" /><span style="margin-left: 8px">分钟</span></el-form-item>
            <el-form-item label="最大登录尝试"><el-input-number v-model="securityConfig.maxLoginAttempts" :min="3" :max="10" /><span style="margin-left: 8px">次</span></el-form-item>
            <el-form-item label="锁定时长"><el-input-number v-model="securityConfig.lockoutDuration" :min="5" :max="60" /><span style="margin-left: 8px">分钟</span></el-form-item>
            <el-form-item label="双因素认证"><el-switch v-model="securityConfig.enableTwoFactor" /></el-form-item>
          </el-form>
        </el-tab-pane>
        
        <el-tab-pane label="通知设置" name="notification">
          <el-form :model="notificationConfig" label-width="120px" style="max-width: 600px">
            <el-form-item label="启用邮件通知"><el-switch v-model="notificationConfig.enableEmail" /></el-form-item>
            <el-form-item label="启用短信通知"><el-switch v-model="notificationConfig.enableSms" /></el-form-item>
            <el-form-item label="启用推送通知"><el-switch v-model="notificationConfig.enablePush" /></el-form-item>
            <el-divider v-if="notificationConfig.enableEmail">邮件服务器配置</el-divider>
            <template v-if="notificationConfig.enableEmail">
              <el-form-item label="SMTP服务器"><el-input v-model="notificationConfig.emailServer" placeholder="smtp.example.com" /></el-form-item>
              <el-form-item label="端口"><el-input-number v-model="notificationConfig.emailPort" :min="1" :max="65535" /></el-form-item>
              <el-form-item label="用户名"><el-input v-model="notificationConfig.emailUsername" /></el-form-item>
            </template>
          </el-form>
        </el-tab-pane>
      </el-tabs>
      
      <div class="form-actions">
        <el-button type="primary" :loading="saving" @click="handleSave">保存设置</el-button>
      </div>
    </el-card>
  </div>
</template>

<style scoped lang="scss">
.configs-view {
  .form-actions { margin-top: 24px; padding-top: 16px; border-top: 1px solid rgba(212,175,55,0.2); }
}
</style>
