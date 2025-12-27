<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Plus, Edit, Delete, Setting } from '@element-plus/icons-vue';
import { settingsApi } from '@/api';
import type { Role } from '@/types';

const loading = ref(false);
const roles = ref<Role[]>([]);
const dialogVisible = ref(false);
const permissionDialogVisible = ref(false);
const dialogTitle = ref('');
const form = ref<Partial<Role>>({});
const currentRole = ref<Role | null>(null);
const saving = ref(false);

// 权限列表
const allPermissions = [
  { group: '员工管理', items: ['employee:read', 'employee:create', 'employee:update', 'employee:delete'] },
  { group: '组织管理', items: ['organization:read', 'organization:create', 'organization:update', 'organization:delete'] },
  { group: '考勤管理', items: ['attendance:read', 'attendance:create', 'attendance:update', 'attendance:delete'] },
  { group: '假期管理', items: ['leave:read', 'leave:create', 'leave:update', 'leave:approve'] },
  { group: '薪酬管理', items: ['payroll:read', 'payroll:create', 'payroll:update', 'payroll:approve'] },
  { group: '报销管理', items: ['expense:read', 'expense:create', 'expense:approve'] },
  { group: '系统设置', items: ['settings:read', 'settings:update', 'settings:admin'] },
];

const fetchData = async () => {
  loading.value = true;
  try {
    const res = await settingsApi.getRoles();
    roles.value = res.data;
  } catch { /* ignore */ } finally { loading.value = false; }
};

const handleAdd = () => {
  form.value = { isActive: true, permissions: [] };
  dialogTitle.value = '新增角色';
  dialogVisible.value = true;
};

const handleEdit = (item: Role) => {
  form.value = { ...item };
  dialogTitle.value = '编辑角色';
  dialogVisible.value = true;
};

const handlePermissions = (item: Role) => {
  currentRole.value = { ...item };
  permissionDialogVisible.value = true;
};

const handleDelete = async (item: Role) => {
  if (item.isSystem) { ElMessage.warning('系统角色不可删除'); return; }
  await ElMessageBox.confirm(`确定删除角色"${item.name}"吗？`, '提示', { type: 'warning' });
  try {
    await settingsApi.deleteRole(item.id);
    ElMessage.success('删除成功');
    fetchData();
  } catch { /* ignore */ }
};

const handleSave = async () => {
  if (!form.value.name || !form.value.code) { ElMessage.warning('请填写必填项'); return; }
  saving.value = true;
  try {
    if (form.value.id) {
      await settingsApi.updateRole(form.value.id, form.value);
    } else {
      await settingsApi.createRole(form.value);
    }
    ElMessage.success('保存成功');
    dialogVisible.value = false;
    fetchData();
  } catch { /* ignore */ } finally { saving.value = false; }
};

const handleSavePermissions = async () => {
  if (!currentRole.value) return;
  saving.value = true;
  try {
    await settingsApi.updateRolePermissions(currentRole.value.id, currentRole.value.permissions || []);
    ElMessage.success('权限已更新');
    permissionDialogVisible.value = false;
    fetchData();
  } catch { /* ignore */ } finally { saving.value = false; }
};

// 权限名称映射
const permissionLabels: Record<string, string> = {
  'employee:read': '查看', 'employee:create': '新增', 'employee:update': '编辑', 'employee:delete': '删除',
  'organization:read': '查看', 'organization:create': '新增', 'organization:update': '编辑', 'organization:delete': '删除',
  'attendance:read': '查看', 'attendance:create': '新增', 'attendance:update': '编辑', 'attendance:delete': '删除',
  'leave:read': '查看', 'leave:create': '申请', 'leave:update': '编辑', 'leave:approve': '审批',
  'payroll:read': '查看', 'payroll:create': '新增', 'payroll:update': '编辑', 'payroll:approve': '审批',
  'expense:read': '查看', 'expense:create': '申请', 'expense:approve': '审批',
  'settings:read': '查看', 'settings:update': '编辑', 'settings:admin': '管理员',
};

onMounted(() => fetchData());
</script>

<template>
  <div class="roles-view">
    <el-card>
      <template #header>
        <div class="card-header">
          <span class="card-title">角色管理</span>
          <el-button type="primary" :icon="Plus" @click="handleAdd">新增角色</el-button>
        </div>
      </template>
      
      <el-table v-loading="loading" :data="roles" stripe>
        <el-table-column prop="code" label="编码" width="120" />
        <el-table-column prop="name" label="名称" width="150" />
        <el-table-column prop="description" label="描述" min-width="200" />
        <el-table-column prop="isSystem" label="系统角色" width="100">
          <template #default="{ row }"><el-tag v-if="row.isSystem" type="warning" size="small">系统</el-tag></template>
        </el-table-column>
        <el-table-column prop="isActive" label="状态" width="80">
          <template #default="{ row }"><el-tag :type="row.isActive ? 'success' : 'danger'" size="small">{{ row.isActive ? '启用' : '禁用' }}</el-tag></template>
        </el-table-column>
        <el-table-column label="操作" width="150" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" size="small" @click="handlePermissions(row)"><el-icon><Setting /></el-icon></el-button>
            <el-button link type="primary" size="small" @click="handleEdit(row)"><el-icon><Edit /></el-icon></el-button>
            <el-button link type="danger" size="small" @click="handleDelete(row)" :disabled="row.isSystem"><el-icon><Delete /></el-icon></el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
    
    <!-- 编辑对话框 -->
    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="500px">
      <el-form :model="form" label-width="80px">
        <el-form-item label="编码" required><el-input v-model="form.code" placeholder="如：admin" :disabled="!!form.id" /></el-form-item>
        <el-form-item label="名称" required><el-input v-model="form.name" placeholder="如：管理员" /></el-form-item>
        <el-form-item label="描述"><el-input v-model="form.description" type="textarea" :rows="2" /></el-form-item>
        <el-form-item label="状态"><el-switch v-model="form.isActive" active-text="启用" inactive-text="禁用" /></el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="saving" @click="handleSave">保存</el-button>
      </template>
    </el-dialog>
    
    <!-- 权限配置对话框 -->
    <el-dialog v-model="permissionDialogVisible" title="配置权限" width="600px">
      <div v-if="currentRole" class="permission-groups">
        <div v-for="group in allPermissions" :key="group.group" class="permission-group">
          <div class="group-title">{{ group.group }}</div>
          <el-checkbox-group v-model="currentRole.permissions">
            <el-checkbox v-for="perm in group.items" :key="perm" :value="perm">{{ permissionLabels[perm] || perm }}</el-checkbox>
          </el-checkbox-group>
        </div>
      </div>
      <template #footer>
        <el-button @click="permissionDialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="saving" @click="handleSavePermissions">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<style scoped lang="scss">
.roles-view {
  .card-header { display: flex; justify-content: space-between; align-items: center;
    .card-title { font-size: 16px; font-weight: 600; }
  }
  .permission-groups {
    .permission-group { margin-bottom: 16px; padding-bottom: 16px; border-bottom: 1px solid rgba(212,175,55,0.2);
      &:last-child { border-bottom: none; }
      .group-title { font-weight: 500; margin-bottom: 8px; color: #D4AF37; }
    }
  }
}
</style>
