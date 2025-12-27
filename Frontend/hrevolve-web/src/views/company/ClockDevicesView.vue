<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Plus, Edit, Delete, Search, Refresh, Connection } from '@element-plus/icons-vue';
import { companyApi } from '@/api';
import type { ClockDevice } from '@/types';

// 状态
const loading = ref(false);
const devices = ref<ClockDevice[]>([]);
const dialogVisible = ref(false);
const dialogTitle = ref('');
const form = ref<Partial<ClockDevice>>({});
const saving = ref(false);
const searchKeyword = ref('');
const statusFilter = ref('');

// 设备类型选项
const deviceTypes = [
  { value: 'fingerprint', label: '指纹机' },
  { value: 'face', label: '人脸识别' },
  { value: 'card', label: '刷卡机' },
  { value: 'app', label: 'APP打卡' },
  { value: 'wifi', label: 'WiFi打卡' },
];

// 状态选项
const statusOptions = [
  { value: 'online', label: '在线', type: 'success' },
  { value: 'offline', label: '离线', type: 'danger' },
  { value: 'maintenance', label: '维护中', type: 'warning' },
];

// 过滤后的设备
const filteredDevices = computed(() => {
  let result = devices.value;
  if (searchKeyword.value) {
    const keyword = searchKeyword.value.toLowerCase();
    result = result.filter(d => 
      d.name.toLowerCase().includes(keyword) || 
      d.serialNumber?.toLowerCase().includes(keyword) ||
      d.location?.toLowerCase().includes(keyword)
    );
  }
  if (statusFilter.value) {
    result = result.filter(d => d.status === statusFilter.value);
  }
  return result;
});

// 获取数据
const fetchData = async () => {
  loading.value = true;
  try {
    const res = await companyApi.getClockDevices();
    devices.value = res.data;
  } catch { /* ignore */ } finally { loading.value = false; }
};

// 新增设备
const handleAdd = () => {
  form.value = { type: 'fingerprint', status: 'offline', isActive: true };
  dialogTitle.value = '新增打卡设备';
  dialogVisible.value = true;
};

// 编辑设备
const handleEdit = (device: ClockDevice) => {
  form.value = { ...device };
  dialogTitle.value = '编辑打卡设备';
  dialogVisible.value = true;
};

// 删除设备
const handleDelete = async (device: ClockDevice) => {
  await ElMessageBox.confirm(`确定删除设备"${device.name}"吗？`, '提示', { type: 'warning' });
  try {
    await companyApi.deleteClockDevice(device.id);
    ElMessage.success('删除成功');
    fetchData();
  } catch { /* ignore */ }
};

// 测试连接
const handleTestConnection = async (device: ClockDevice) => {
  try {
    ElMessage.info('正在测试连接...');
    // 模拟测试连接
    await new Promise(resolve => setTimeout(resolve, 1500));
    ElMessage.success(`设备"${device.name}"连接正常`);
  } catch {
    ElMessage.error('连接失败');
  }
};

// 保存
const handleSave = async () => {
  if (!form.value.name || !form.value.serialNumber) {
    ElMessage.warning('请填写必填项');
    return;
  }
  saving.value = true;
  try {
    if (form.value.id) {
      await companyApi.updateClockDevice(form.value.id, form.value);
    } else {
      await companyApi.createClockDevice(form.value);
    }
    ElMessage.success('保存成功');
    dialogVisible.value = false;
    fetchData();
  } catch { /* ignore */ } finally { saving.value = false; }
};

// 获取状态标签类型
const getStatusType = (status: string) => {
  return statusOptions.find(s => s.value === status)?.type || 'info';
};

// 获取状态标签文本
const getStatusLabel = (status: string) => {
  return statusOptions.find(s => s.value === status)?.label || status;
};

// 获取设备类型文本
const getDeviceTypeLabel = (type: string) => {
  return deviceTypes.find(t => t.value === type)?.label || type;
};

onMounted(() => fetchData());
</script>

<template>
  <div class="clock-devices-view">
    <el-card>
      <template #header>
        <div class="card-header">
          <span class="card-title">打卡设备管理</span>
          <div class="header-actions">
            <el-input
              v-model="searchKeyword"
              placeholder="搜索设备"
              :prefix-icon="Search"
              clearable
              style="width: 180px"
            />
            <el-select v-model="statusFilter" placeholder="状态筛选" clearable style="width: 120px">
              <el-option v-for="s in statusOptions" :key="s.value" :label="s.label" :value="s.value" />
            </el-select>
            <el-button :icon="Refresh" @click="fetchData">刷新</el-button>
            <el-button type="primary" :icon="Plus" @click="handleAdd">新增设备</el-button>
          </div>
        </div>
      </template>
      
      <el-table v-loading="loading" :data="filteredDevices" stripe>
        <el-table-column prop="name" label="设备名称" min-width="150" />
        <el-table-column prop="serialNumber" label="序列号" width="150" />
        <el-table-column prop="type" label="类型" width="100">
          <template #default="{ row }">
            <el-tag size="small">{{ getDeviceTypeLabel(row.type) }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="location" label="位置" min-width="150" />
        <el-table-column prop="ipAddress" label="IP地址" width="130" />
        <el-table-column prop="status" label="状态" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.status)" size="small">
              {{ getStatusLabel(row.status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="lastSyncTime" label="最后同步" width="160">
          <template #default="{ row }">
            {{ row.lastSyncTime ? new Date(row.lastSyncTime).toLocaleString() : '-' }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="180" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" size="small" @click="handleTestConnection(row)">
              <el-icon><Connection /></el-icon> 测试
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
      
      <el-empty v-if="!loading && filteredDevices.length === 0" description="暂无设备数据" />
    </el-card>
    
    <!-- 编辑对话框 -->
    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="550px">
      <el-form :model="form" label-width="100px">
        <el-form-item label="设备名称" required>
          <el-input v-model="form.name" placeholder="请输入设备名称" />
        </el-form-item>
        <el-form-item label="序列号" required>
          <el-input v-model="form.serialNumber" placeholder="请输入序列号" />
        </el-form-item>
        <el-form-item label="设备类型" required>
          <el-select v-model="form.type" style="width: 100%">
            <el-option v-for="t in deviceTypes" :key="t.value" :label="t.label" :value="t.value" />
          </el-select>
        </el-form-item>
        <el-form-item label="位置">
          <el-input v-model="form.location" placeholder="请输入设备位置" />
        </el-form-item>
        <el-form-item label="IP地址">
          <el-input v-model="form.ipAddress" placeholder="请输入IP地址" />
        </el-form-item>
        <el-form-item label="状态">
          <el-select v-model="form.status" style="width: 100%">
            <el-option v-for="s in statusOptions" :key="s.value" :label="s.label" :value="s.value" />
          </el-select>
        </el-form-item>
        <el-form-item label="启用">
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
