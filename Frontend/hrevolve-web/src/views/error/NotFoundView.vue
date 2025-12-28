<script setup lang="ts">
import { useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';

const router = useRouter();
const { t } = useI18n();
</script>

<template>
  <div class="not-found">
    <h1>{{ t('errorPage.notFoundTitle') }}</h1>
    <p>{{ t('errorPage.notFoundDesc') }}</p>
    <el-button type="primary" @click="router.push('/')">{{ t('errorPage.backHome') }}</el-button>
  </div>
</template>

<style scoped lang="scss">
.not-found {
  height: 100vh;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  
  h1 { font-size: 120px; color: #D4AF37; margin: 0; }
  p { font-size: 18px; color: rgba(255,255,255,0.65); margin: 16px 0 24px; }
}
</style>
