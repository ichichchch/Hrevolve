<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Plus, Edit, Delete } from '@element-plus/icons-vue';
import { scheduleApi } from '@/api';
import type { ShiftTemplate } from '@/types';

const { t } = useI18n();
const loading = ref(false);
const templates = ref<ShiftTemplate[]>([]);
const dialogVisible = ref(false);
const dialogTitle = ref('');
const form = ref<Partial<ShiftTemplate>>({});
const saving = ref(false);

// 获取数据
const fetchData = async () => {
  loading.value = true;
  try {
    const res = await scheduleApi.getShiftTemplates();
    templates.value = res.data;
  } catch { /* ignore */ } finally { loading.value = false; }
};

// 新增
const handleAdd = () => {
  form.value = { isActive: true, workHours: 8, breakMinutes: 60 };
  dialogTitle.value = t('schedule.newShift');
  dialogVisible.value = true;
};

// 编辑
const handleEdit = (item: ShiftTemplate) => {
  form.value = { ...item };
  dialogTitle.value = t('schedule.editShift');
  dialogVisible.value = true;
};

// 删除
const handleDelete = async (item: ShiftTemplate) => {
  await ElMessageBox.confirm(t('schedule.confirmDeleteShift', { name: item.name }), t('common.confirm'), { type: 'warning' });
  try {
    await scheduleApi.deleteShiftTemplate(item.id);
    ElMessage.success(t('common.success'));
    fetchData();
  } catch { /* ignore */ }
};

// 保存
const handleSave = async () => {
  if (!form.value.name || !form.value.startTime || !form.value.endTime) {
    ElMessage.warning(t('tax.fillRequired'));
    return;
  }
  saving.value = true;
  try {
    if (form.value.id) {
      await scheduleApi.updateShiftTemplate(form.value.id, form.value);
    } else {
      await scheduleApi.createShiftTemplate(form.value);
    }
    ElMessage.success(t('common.success'));
    dialogVisible.value = false;
    fetchData();
  } catch { /* ignore */ } finally { saving.value = false; }
};

onMounted(() => fetchData());
</script>

<template>
  <div class="shift-templates-view">
    <el-card>
      <template #header>
        <div class="card-header">
          <span class="card-title">{{ t('schedule.shiftTemplates') }}</span>
          <el-button type="primary" :icon="Plus" @click="handleAdd">{{ t('schedule.addShift') }}</el-button>
        </div>
      </template>
      
      <el-row :gutter="16" v-loading="loading">
        <el-col v-for="item in templates" :key="item.id" :xs="24" :sm="12" :md="8" :lg="6">
          <el-card class="template-card" shadow="hover">
            <div class="template-header">
              <span class="template-name">{{ item.name }}</span>
              <el-tag v-if="!item.isActive" type="danger" size="small">{{ t('settings.disabled') }}</el-tag>
            </div>
            <div class="template-time">
              <span class="time-label">{{ t('schedule.workTime') }}</span>
              <span class="time-value">{{ item.startTime }} - {{ item.endTime }}</span>
            </div>
            <div class="template-info">
              <span>{{ t('schedule.workHours') }}: {{ item.workHours }}h</span>
              <span>{{ t('schedule.breakMinutes') }}: {{ item.breakMinutes }}min</span>
            </div>
            <div class="template-actions">
              <el-button link type="primary" size="small" @click="handleEdit(item)">
                <el-icon><Edit /></el-icon> {{ t('common.edit') }}
              </el-button>
              <el-button link type="danger" size="small" @click="handleDelete(item)">
                <el-icon><Delete /></el-icon> {{ t('common.delete') }}
              </el-button>
            </div>
          </el-card>
        </el-col>
      </el-row>
      
      <el-empty v-if="!loading && templates.length === 0" :description="t('schedule.noShiftTemplates')" />
    </el-card>
    
    <!-- 编辑对话框 -->
    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="500px">
      <el-form :model="form" label-width="100px">
        <el-form-item :label="t('schedule.shiftName')" required>
          <el-input v-model="form.name" :placeholder="t('schedule.shiftNamePlaceholder')" />
        </el-form-item>
        <el-form-item :label="t('schedule.startTime')" required>
          <el-time-picker v-model="form.startTime" format="HH:mm" value-format="HH:mm" style="width: 100%" />
        </el-form-item>
        <el-form-item :label="t('schedule.endTime')" required>
          <el-time-picker v-model="form.endTime" format="HH:mm" value-format="HH:mm" style="width: 100%" />
        </el-form-item>
        <el-form-item :label="t('schedule.workHours')">
          <el-input-number v-model="form.workHours" :min="1" :max="24" />
          <span style="margin-left: 8px">{{ t('schedule.hours') }}</span>
        </el-form-item>
        <el-form-item :label="t('schedule.breakMinutes')">
          <el-input-number v-model="form.breakMinutes" :min="0" :max="180" :step="15" />
          <span style="margin-left: 8px">{{ t('settings.minutes') }}</span>
        </el-form-item>
        <el-form-item :label="t('common.status')">
          <el-switch v-model="form.isActive" :active-text="t('settings.enabled')" :inactive-text="t('settings.disabled')" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">{{ t('common.cancel') }}</el-button>
        <el-button type="primary" :loading="saving" @click="handleSave">{{ t('common.save') }}</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<style scoped lang="scss">
.shift-templates-view {
  .card-header { display: flex; justify-content: space-between; align-items: center;
    .card-title { font-size: 16px; font-weight: 600; }
  }
  .template-card {
    margin-bottom: 16px;
    .template-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 12px;
      .template-name { font-size: 16px; font-weight: 600; color: #D4AF37; }
    }
    .template-time { margin-bottom: 8px;
      .time-label { font-size: 12px; color: rgba(255,255,255,0.5); margin-right: 8px; }
      .time-value { font-size: 14px; font-weight: 500; }
    }
    .template-info { font-size: 13px; color: rgba(255,255,255,0.65); display: flex; gap: 16px; margin-bottom: 12px; }
    .template-actions { display: flex; gap: 8px; border-top: 1px solid rgba(212,175,55,0.2); padding-top: 12px; }
  }
}
</style>
