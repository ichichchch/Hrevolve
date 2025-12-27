<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Plus, Edit, Delete } from '@element-plus/icons-vue';
import { scheduleApi } from '@/api';
import type { ShiftTemplate } from '@/types';

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
  dialogTitle.value = '新增班次模板';
  dialogVisible.value = true;
};

// 编辑
const handleEdit = (item: ShiftTemplate) => {
  form.value = { ...item };
  dialogTitle.value = '编辑班次模板';
  dialogVisible.value = true;
};

// 删除
const handleDelete = async (item: ShiftTemplate) => {
  await ElMessageBox.confirm(`确定删除班次"${item.name}"吗？`, '提示', { type: 'warning' });
  try {
    await scheduleApi.deleteShiftTemplate(item.id);
    ElMessage.success('删除成功');
    fetchData();
  } catch { /* ignore */ }
};

// 保存
const handleSave = async () => {
  if (!form.value.name || !form.value.startTime || !form.value.endTime) {
    ElMessage.warning('请填写必填项');
    return;
  }
  saving.value = true;
  try {
    if (form.value.id) {
      await scheduleApi.updateShiftTemplate(form.value.id, form.value);
    } else {
      await scheduleApi.createShiftTemplate(form.value);
    }
    ElMessage.success('保存成功');
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
          <span class="card-title">班次模板管理</span>
          <el-button type="primary" :icon="Plus" @click="handleAdd">新增班次</el-button>
        </div>
      </template>
      
      <el-row :gutter="16" v-loading="loading">
        <el-col v-for="item in templates" :key="item.id" :xs="24" :sm="12" :md="8" :lg="6">
          <el-card class="template-card" shadow="hover">
            <div class="template-header">
              <span class="template-name">{{ item.name }}</span>
              <el-tag v-if="!item.isActive" type="danger" size="small">已禁用</el-tag>
            </div>
            <div class="template-time">
              <span class="time-label">工作时间</span>
              <span class="time-value">{{ item.startTime }} - {{ item.endTime }}</span>
            </div>
            <div class="template-info">
              <span>工时: {{ item.workHours }}h</span>
              <span>休息: {{ item.breakMinutes }}min</span>
            </div>
            <div class="template-actions">
              <el-button link type="primary" size="small" @click="handleEdit(item)">
                <el-icon><Edit /></el-icon> 编辑
              </el-button>
              <el-button link type="danger" size="small" @click="handleDelete(item)">
                <el-icon><Delete /></el-icon> 删除
              </el-button>
            </div>
          </el-card>
        </el-col>
      </el-row>
      
      <el-empty v-if="!loading && templates.length === 0" description="暂无班次模板" />
    </el-card>
    
    <!-- 编辑对话框 -->
    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="500px">
      <el-form :model="form" label-width="100px">
        <el-form-item label="班次名称" required>
          <el-input v-model="form.name" placeholder="如：早班、晚班" />
        </el-form-item>
        <el-form-item label="上班时间" required>
          <el-time-picker v-model="form.startTime" format="HH:mm" value-format="HH:mm" style="width: 100%" />
        </el-form-item>
        <el-form-item label="下班时间" required>
          <el-time-picker v-model="form.endTime" format="HH:mm" value-format="HH:mm" style="width: 100%" />
        </el-form-item>
        <el-form-item label="工作时长">
          <el-input-number v-model="form.workHours" :min="1" :max="24" />
          <span style="margin-left: 8px">小时</span>
        </el-form-item>
        <el-form-item label="休息时长">
          <el-input-number v-model="form.breakMinutes" :min="0" :max="180" :step="15" />
          <span style="margin-left: 8px">分钟</span>
        </el-form-item>
        <el-form-item label="状态">
          <el-switch v-model="form.isActive" active-text="启用" inactive-text="禁用" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="saving" @click="handleSave">保存</el-button>
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
