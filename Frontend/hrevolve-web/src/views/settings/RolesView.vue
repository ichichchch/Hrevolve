<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Plus, Edit, Delete, Setting } from '@element-plus/icons-vue';
import { settingsApi } from '@/api';
import type { Role } from '@/types';

const { t } = useI18n();
const loading = ref(false);
const roles = ref<Role[]>([]);
const dialogVisible = ref(false);
const permissionDialogVisible = ref(false);
const dialogTitle = ref('');
const form = ref<Partial<Role>>({});
const currentRole = ref<Role | null>(null);
const saving = ref(false);

// 权限列表 - 使用 computed 实现响应式翻译
const allPermissions = computed(() => [
  { group: t('settings.permissionGroups.employee'), items: ['employee:read', 'employee:create', 'employee:update', 'employee:delete'] },
  { group: t('settings.permissionGroups.organization'), items: ['organization:read', 'organization:create', 'organization:update', 'organization:delete'] },
  { group: t('settings.permissionGroups.attendance'), items: ['attendance:read', 'attendance:create', 'attendance:update', 'attendance:delete'] },
  { group: t('settings.permissionGroups.leave'), items: ['leave:read', 'leave:create', 'leave:update', 'leave:approve'] },
  { group: t('settings.permissionGroups.payroll'), items: ['payroll:read', 'payroll:create', 'payroll:update', 'payroll:approve'] },
  { group: t('settings.permissionGroups.expense'), items: ['expense:read', 'expense:create', 'expense:approve'] },
  { group: t('settings.permissionGroups.settings'), items: ['settings:read', 'settings:update', 'settings:admin'] },
]);

const fetchData = async () => {
  loading.value = true;
  try {
    const res = await settingsApi.getRoles();
    roles.value = res.data;
  } catch { /* ignore */ } finally { loading.value = false; }
};

const handleAdd = () => {
  form.value = { isActive: true, permissions: [] };
  dialogTitle.value = t('settings.newRole');
  dialogVisible.value = true;
};

const handleEdit = (item: Role) => {
  form.value = { ...item };
  dialogTitle.value = t('settings.editRole');
  dialogVisible.value = true;
};

const handlePermissions = (item: Role) => {
  currentRole.value = { ...item };
  permissionDialogVisible.value = true;
};

const handleDelete = async (item: Role) => {
  if (item.isSystem) { ElMessage.warning(t('settings.systemRoleCannotDelete')); return; }
  await ElMessageBox.confirm(t('settings.confirmDeleteRole', { name: item.name }), t('common.confirm'), { type: 'warning' });
  try {
    await settingsApi.deleteRole(item.id);
    ElMessage.success(t('common.success'));
    fetchData();
  } catch { /* ignore */ }
};

const handleSave = async () => {
  if (!form.value.name || !form.value.code) { ElMessage.warning(t('tax.fillRequired')); return; }
  saving.value = true;
  try {
    if (form.value.id) {
      await settingsApi.updateRole(form.value.id, form.value);
    } else {
      await settingsApi.createRole(form.value);
    }
    ElMessage.success(t('common.success'));
    dialogVisible.value = false;
    fetchData();
  } catch { /* ignore */ } finally { saving.value = false; }
};

const handleSavePermissions = async () => {
  if (!currentRole.value) return;
  saving.value = true;
  try {
    await settingsApi.updateRolePermissions(currentRole.value.id, currentRole.value.permissions || []);
    ElMessage.success(t('settings.permissionsUpdated'));
    permissionDialogVisible.value = false;
    fetchData();
  } catch { /* ignore */ } finally { saving.value = false; }
};

// 权限名称映射 - 使用 computed 实现响应式翻译
const permissionLabels = computed(() => ({
  'employee:read': t('settings.permissionLabels.read'), 'employee:create': t('settings.permissionLabels.create'), 'employee:update': t('settings.permissionLabels.update'), 'employee:delete': t('settings.permissionLabels.delete'),
  'organization:read': t('settings.permissionLabels.read'), 'organization:create': t('settings.permissionLabels.create'), 'organization:update': t('settings.permissionLabels.update'), 'organization:delete': t('settings.permissionLabels.delete'),
  'attendance:read': t('settings.permissionLabels.read'), 'attendance:create': t('settings.permissionLabels.create'), 'attendance:update': t('settings.permissionLabels.update'), 'attendance:delete': t('settings.permissionLabels.delete'),
  'leave:read': t('settings.permissionLabels.read'), 'leave:create': t('settings.permissionLabels.apply'), 'leave:update': t('settings.permissionLabels.update'), 'leave:approve': t('settings.permissionLabels.approve'),
  'payroll:read': t('settings.permissionLabels.read'), 'payroll:create': t('settings.permissionLabels.create'), 'payroll:update': t('settings.permissionLabels.update'), 'payroll:approve': t('settings.permissionLabels.approve'),
  'expense:read': t('settings.permissionLabels.read'), 'expense:create': t('settings.permissionLabels.apply'), 'expense:approve': t('settings.permissionLabels.approve'),
  'settings:read': t('settings.permissionLabels.read'), 'settings:update': t('settings.permissionLabels.update'), 'settings:admin': t('settings.permissionLabels.admin'),
} as Record<string, string>));

onMounted(() => fetchData());
</script>

<template>
  <div class="roles-view">
    <el-card>
      <template #header>
        <div class="card-header">
          <span class="card-title">{{ t('settings.roleManagement') }}</span>
          <el-button type="primary" :icon="Plus" @click="handleAdd">{{ t('settings.addRole') }}</el-button>
        </div>
      </template>
      
      <el-table v-loading="loading" :data="roles" stripe>
        <el-table-column prop="code" :label="t('settings.code')" width="120" />
        <el-table-column prop="name" :label="t('settings.name')" width="150" />
        <el-table-column prop="description" :label="t('settings.description')" min-width="200" />
        <el-table-column prop="isSystem" :label="t('settings.systemRole')" width="100">
          <template #default="{ row }"><el-tag v-if="row.isSystem" type="warning" size="small">{{ t('settings.system') }}</el-tag></template>
        </el-table-column>
        <el-table-column prop="isActive" :label="t('common.status')" width="80">
          <template #default="{ row }"><el-tag :type="row.isActive ? 'success' : 'danger'" size="small">{{ row.isActive ? t('settings.enabled') : t('settings.disabled') }}</el-tag></template>
        </el-table-column>
        <el-table-column :label="t('common.actions')" width="150" fixed="right">
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
        <el-form-item :label="t('settings.code')" required><el-input v-model="form.code" :placeholder="t('settings.codePlaceholder')" :disabled="!!form.id" /></el-form-item>
        <el-form-item :label="t('settings.name')" required><el-input v-model="form.name" :placeholder="t('settings.namePlaceholder')" /></el-form-item>
        <el-form-item :label="t('settings.description')"><el-input v-model="form.description" type="textarea" :rows="2" /></el-form-item>
        <el-form-item :label="t('common.status')"><el-switch v-model="form.isActive" :active-text="t('settings.enabled')" :inactive-text="t('settings.disabled')" /></el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">{{ t('common.cancel') }}</el-button>
        <el-button type="primary" :loading="saving" @click="handleSave">{{ t('common.save') }}</el-button>
      </template>
    </el-dialog>
    
    <!-- 权限配置对话框 -->
    <el-dialog v-model="permissionDialogVisible" :title="t('settings.configPermissions')" width="600px">
      <div v-if="currentRole" class="permission-groups">
        <div v-for="group in allPermissions" :key="group.group" class="permission-group">
          <div class="group-title">{{ group.group }}</div>
          <el-checkbox-group v-model="currentRole.permissions">
            <el-checkbox v-for="perm in group.items" :key="perm" :value="perm">{{ permissionLabels[perm] || perm }}</el-checkbox>
          </el-checkbox-group>
        </div>
      </div>
      <template #footer>
        <el-button @click="permissionDialogVisible = false">{{ t('common.cancel') }}</el-button>
        <el-button type="primary" :loading="saving" @click="handleSavePermissions">{{ t('common.save') }}</el-button>
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
