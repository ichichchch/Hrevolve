<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { ElMessage } from 'element-plus';
import { settingsApi } from '@/api';

const { t } = useI18n();
const loading = ref(false);
const saving = ref(false);
const activeTab = ref('general');

// 语言选项（响应式）
const languageOptions = computed(() => [
  { label: t('languageOptions.zhCN'), value: 'zh-CN' },
  { label: t('languageOptions.enUS'), value: 'en-US' },
]);

// 货币选项（响应式）
const currencyOptions = computed(() => [
  { label: t('currencyOptions.CNY'), value: 'CNY' },
  { label: t('currencyOptions.USD'), value: 'USD' },
  { label: t('currencyOptions.EUR'), value: 'EUR' },
]);

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
    ElMessage.success(t('common.success'));
  } catch { /* ignore */ } finally { saving.value = false; }
};

onMounted(() => fetchData());
</script>

<template>
  <div class="configs-view">
    <el-card v-loading="loading">
      <el-tabs v-model="activeTab">
        <el-tab-pane :label="t('settings.generalSettings')" name="general">
          <el-form :model="generalConfig" label-width="120px" style="max-width: 600px">
            <el-form-item :label="t('settings.systemName')"><el-input v-model="generalConfig.systemName" /></el-form-item>
            <el-form-item :label="t('settings.companyName')"><el-input v-model="generalConfig.companyName" /></el-form-item>
            <el-form-item :label="t('settings.timezone')">
              <el-select v-model="generalConfig.timezone" style="width: 100%">
                <el-option label="Asia/Shanghai (UTC+8)" value="Asia/Shanghai" />
                <el-option label="Asia/Tokyo (UTC+9)" value="Asia/Tokyo" />
                <el-option label="America/New_York (UTC-5)" value="America/New_York" />
                <el-option label="Europe/London (UTC+0)" value="Europe/London" />
              </el-select>
            </el-form-item>
            <el-form-item :label="t('settings.dateFormat')">
              <el-select v-model="generalConfig.dateFormat" style="width: 100%">
                <el-option label="YYYY-MM-DD" value="YYYY-MM-DD" />
                <el-option label="DD/MM/YYYY" value="DD/MM/YYYY" />
                <el-option label="MM/DD/YYYY" value="MM/DD/YYYY" />
              </el-select>
            </el-form-item>
            <el-form-item :label="t('settings.defaultLanguage')">
              <el-select v-model="generalConfig.language" style="width: 100%">
                <el-option v-for="opt in languageOptions" :key="opt.value" :label="opt.label" :value="opt.value" />
              </el-select>
            </el-form-item>
            <el-form-item :label="t('settings.currency')">
              <el-select v-model="generalConfig.currency" style="width: 100%">
                <el-option v-for="opt in currencyOptions" :key="opt.value" :label="opt.label" :value="opt.value" />
              </el-select>
            </el-form-item>
          </el-form>
        </el-tab-pane>
        
        <el-tab-pane :label="t('settings.securitySettings')" name="security">
          <el-form :model="securityConfig" label-width="140px" style="max-width: 600px">
            <el-form-item :label="t('settings.passwordMinLength')"><el-input-number v-model="securityConfig.passwordMinLength" :min="6" :max="20" /></el-form-item>
            <el-form-item :label="t('settings.requireUppercase')"><el-switch v-model="securityConfig.passwordRequireUppercase" /></el-form-item>
            <el-form-item :label="t('settings.requireLowercase')"><el-switch v-model="securityConfig.passwordRequireLowercase" /></el-form-item>
            <el-form-item :label="t('settings.requireNumber')"><el-switch v-model="securityConfig.passwordRequireNumber" /></el-form-item>
            <el-form-item :label="t('settings.requireSpecial')"><el-switch v-model="securityConfig.passwordRequireSpecial" /></el-form-item>
            <el-divider />
            <el-form-item :label="t('settings.sessionTimeout')"><el-input-number v-model="securityConfig.sessionTimeout" :min="5" :max="120" /><span style="margin-left: 8px">{{ t('settings.minutes') }}</span></el-form-item>
            <el-form-item :label="t('settings.maxLoginAttempts')"><el-input-number v-model="securityConfig.maxLoginAttempts" :min="3" :max="10" /><span style="margin-left: 8px">{{ t('settings.times') }}</span></el-form-item>
            <el-form-item :label="t('settings.lockoutDuration')"><el-input-number v-model="securityConfig.lockoutDuration" :min="5" :max="60" /><span style="margin-left: 8px">{{ t('settings.minutes') }}</span></el-form-item>
            <el-form-item :label="t('settings.twoFactorAuth')"><el-switch v-model="securityConfig.enableTwoFactor" /></el-form-item>
          </el-form>
        </el-tab-pane>
        
        <el-tab-pane :label="t('settings.notificationSettings')" name="notification">
          <el-form :model="notificationConfig" label-width="120px" style="max-width: 600px">
            <el-form-item :label="t('settings.enableEmailNotification')"><el-switch v-model="notificationConfig.enableEmail" /></el-form-item>
            <el-form-item :label="t('settings.enableSmsNotification')"><el-switch v-model="notificationConfig.enableSms" /></el-form-item>
            <el-form-item :label="t('settings.enablePushNotification')"><el-switch v-model="notificationConfig.enablePush" /></el-form-item>
            <el-divider v-if="notificationConfig.enableEmail">{{ t('settings.emailServerConfig') }}</el-divider>
            <template v-if="notificationConfig.enableEmail">
              <el-form-item :label="t('settings.smtpServer')"><el-input v-model="notificationConfig.emailServer" placeholder="smtp.example.com" /></el-form-item>
              <el-form-item :label="t('settings.port')"><el-input-number v-model="notificationConfig.emailPort" :min="1" :max="65535" /></el-form-item>
              <el-form-item :label="t('settings.username')"><el-input v-model="notificationConfig.emailUsername" /></el-form-item>
            </template>
          </el-form>
        </el-tab-pane>
      </el-tabs>
      
      <div class="form-actions">
        <el-button type="primary" :loading="saving" @click="handleSave">{{ t('tax.saveSettings') }}</el-button>
      </div>
    </el-card>
  </div>
</template>

<style scoped lang="scss">
.configs-view {
  .form-actions { margin-top: 24px; padding-top: 16px; border-top: 1px solid rgba(212,175,55,0.2); }
}
</style>
