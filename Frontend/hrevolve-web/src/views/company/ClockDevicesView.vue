<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Plus, Edit, Delete, Search, Refresh, Connection } from '@element-plus/icons-vue';
import { companyApi } from '@/api';
import type { ClockDevice } from '@/types';

const { t } = useI18n();

// 状态
const loading = ref(false);
const devices = ref<ClockDevice[]>([]);
const dialogVisible = ref(false);
const dialogTitle = ref('');
const form = ref<Partial<ClockDevice>>({});
const saving = ref(false);
const searchKeyword = ref('');
const typeFilter = ref('');

// 设备类型选项（响应式）- 使用与类型定义匹配的值
const deviceTypes = computed(() => [
  { value: 'Fingerprint', label: t('clockDevices.typeFingerprint') },
  { value: 'Face', label: t('clockDevices.typeFace') },
  { value: 'Card', label: t('clockDevices.typeCard') },
  { value: 'GPS', label: t('clockDevices.typeApp') },
  { value: 'WiFi', label: t('clockDevices.typeWifi') },
]);

// 过滤后的设备
const filteredDevices = computed(() => {
  let result = devices.value;
  if (searchKeyword.value) {
    const keyword = searchKeyword.value.toLowerCase();
    result = result.filter(d => 
      d.name.toLowerCase().includes(keyword) || 
      d.code?.toLowerCase().includes(keyword) ||
      d.location?.toLowerCase().includes(keyword)
    );
  }
  if (typeFilter.value) {
    result = result.filter(d => d.type === typeFilter.value);
  }
  return result;
});

// 获取数据
const fetchData = async () => {
  loading.value = true;
  try {
    const res = await companyApi.getClockDevices();
    // 处理分页响应
    devices.value = Array.isArray(res.data) ? res.data : (res.data as any).items || [];
  } catch { /* ignore */ } finally { loading.value = false; }
};

// 新增设备
const handleAdd = () => {
  form.value = { type: 'Fingerprint', isActive: true };
  dialogTitle.value = t('clockDevices.addDevice');
  dialogVisible.value = true;
};

// 编辑设备
const handleEdit = (device: ClockDevice) => {
  form.value = { ...device };
  dialogTitle.value = t('clockDevices.editDevice');
  dialogVisible.value = true;
};

// 删除设备
const handleDelete = async (device: ClockDevice) => {
  await ElMessageBox.confirm(t('clockDevices.confirmDelete', { name: device.name }), t('assistantExtra.tip'), { type: 'warning' });
  try {
    await companyApi.deleteClockDevice(device.id);
    ElMessage.success(t('common.success'));
    fetchData();
  } catch { /* ignore */ }
};

// 测试连接
const handleTestConnection = async (device: ClockDevice) => {
  try {
    ElMessage.info(t('clockDevices.testConnection'));
    // 模拟测试连接
    await new Promise(resolve => setTimeout(resolve, 1500));
    ElMessage.success(t('clockDevices.connectionSuccess', { name: device.name }));
  } catch {
    ElMessage.error(t('clockDevices.connectionFailed'));
  }
};

// 保存
const handleSave = async () => {
  if (!form.value.name || !form.value.code) {
    ElMessage.warning(t('clockDevices.fillRequired'));
    return;
  }
  saving.value = true;
  try {
    if (form.value.id) {
      await companyApi.updateClockDevice(form.value.id, form.value);
    } else {
      await companyApi.createClockDevice(form.value);
    }
    ElMessage.success(t('common.success'));
    dialogVisible.value = false;
    fetchData();
  } catch { /* ignore */ } finally { saving.value = false; }
};

// 获取设备类型文本
const getDeviceTypeLabel = (type: string) => {
  return deviceTypes.value.find(t => t.value === type)?.label || type;
};

onMounted(() => fetchData());
</script>

<template>
  <div class="clock-devices-view">
    <el-card>
      <template #header>
        <div class="card-header">
          <span class="card-title">{{ t('clockDevices.management') }}</span>
          <div class="header-actions">
            <el-input
              v-model="searchKeyword"
              :placeholder="t('clockDevices.searchPlaceholder')"
              :prefix-icon="Search"
              clearable
              style="width: 180px"
            />
            <el-select v-model="typeFilter" :placeholder="t('clockDevices.deviceType')" clearable style="width: 120px">
              <el-option v-for="dt in deviceTypes" :key="dt.value" :label="dt.label" :value="dt.value" />
            </el-select>
            <el-button :icon="Refresh" @click="fetchData">{{ t('clockDevices.refresh') }}</el-button>
            <el-button type="primary" :icon="Plus" @click="handleAdd">{{ t('clockDevices.addDevice') }}</el-button>
          </div>
        </div>
      </template>
      
      <el-table v-loading="loading" :data="filteredDevices" stripe>
        <el-table-column prop="code" label="Code" width="120" />
        <el-table-column prop="name" :label="t('clockDevices.deviceName')" min-width="150" />
        <el-table-column prop="type" :label="t('clockDevices.deviceType')" width="120">
          <template #default="{ row }">
            <el-tag size="small">{{ getDeviceTypeLabel(row.type) }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="location" :label="t('clockDevices.location')" min-width="150" />
        <el-table-column prop="ipAddress" :label="t('clockDevices.ipAddress')" width="130" />
        <el-table-column prop="isActive" :label="t('common.status')" width="100">
          <template #default="{ row }">
            <el-tag :type="row.isActive ? 'success' : 'danger'" size="small">
              {{ row.isActive ? t('settings.enabled') : t('settings.disabled') }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="lastSyncAt" :label="t('clockDevices.lastSync')" width="160">
          <template #default="{ row }">
            {{ row.lastSyncAt ? new Date(row.lastSyncAt).toLocaleString() : '-' }}
          </template>
        </el-table-column>
        <el-table-column :label="t('common.actions')" width="180" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" size="small" @click="handleTestConnection(row)">
              <el-icon><Connection /></el-icon> {{ t('clockDevices.test') }}
            </el-button>
            <el-button link type="primary" size="small" @click="handleEdit(row)">
              <el-icon><Edit /></el-icon>
            </el-button>
            <el-button link type="danger" size="small" @click="handleDelete(row)">
              <el-icon><Delete /></el-icon>
            </el-button>
          </template>
        </el-table-column>
      </el-table>
      
      <el-empty v-if="!loading && filteredDevices.length === 0" :description="t('clockDevices.noData')" />
    </el-card>
    
    <!-- 编辑对话框 -->
    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="550px">
      <el-form :model="form" label-width="100px">
        <el-form-item label="Code" required>
          <el-input v-model="form.code" placeholder="e.g., DEV001" />
        </el-form-item>
        <el-form-item :label="t('clockDevices.deviceName')" required>
          <el-input v-model="form.name" :placeholder="t('clockDevices.deviceNamePlaceholder')" />
        </el-form-item>
        <el-form-item :label="t('clockDevices.deviceType')" required>
          <el-select v-model="form.type" style="width: 100%">
            <el-option v-for="dt in deviceTypes" :key="dt.value" :label="dt.label" :value="dt.value" />
          </el-select>
        </el-form-item>
        <el-form-item :label="t('clockDevices.location')">
          <el-input v-model="form.location" :placeholder="t('clockDevices.locationPlaceholder')" />
        </el-form-item>
        <el-form-item :label="t('clockDevices.ipAddress')">
          <el-input v-model="form.ipAddress" :placeholder="t('clockDevices.ipAddressPlaceholder')" />
        </el-form-item>
        <el-form-item :label="t('settings.enabled')">
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
.clock-devices-view {
  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    flex-wrap: wrap;
    gap: 12px;
    .card-title { font-size: 16px; font-weight: 600; }
    .header-actions { display: flex; gap: 12px; flex-wrap: wrap; }
  }
}
</style>
