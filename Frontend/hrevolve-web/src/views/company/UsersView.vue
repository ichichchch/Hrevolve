<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Plus, Edit, Delete, Search, Refresh, Key } from '@element-plus/icons-vue';
import { settingsApi } from '@/api';
import type { User, Role } from '@/types';

const { t } = useI18n();

// 状态
const loading = ref(false);
const users = ref<User[]>([]);
const roles = ref<Role[]>([]);
const dialogVisible = ref(false);
const dialogTitle = ref('');
const form = ref<Partial<User>>({});
const saving = ref(false);
const searchKeyword = ref('');
const roleFilter = ref('');
const pagination = ref({ page: 1, pageSize: 20, total: 0 });

// 过滤后的用户
const filteredUsers = computed(() => {
  let result = users.value;
  if (searchKeyword.value) {
    const keyword = searchKeyword.value.toLowerCase();
    result = result.filter(u => 
      u.username?.toLowerCase().includes(keyword) || 
      u.displayName?.toLowerCase().includes(keyword) ||
      u.email?.toLowerCase().includes(keyword)
    );
  }
  if (roleFilter.value) {
    result = result.filter(u => u.roles?.includes(roleFilter.value));
  }
  return result;
});

// 获取数据
const fetchData = async () => {
  loading.value = true;
  try {
    const [usersRes, rolesRes] = await Promise.all([
      settingsApi.getUsers({ page: pagination.value.page, pageSize: pagination.value.pageSize }),
      settingsApi.getRoles()
    ]);
    users.value = usersRes.data.items || usersRes.data;
    pagination.value.total = usersRes.data.total || users.value.length;
    roles.value = rolesRes.data;
  } catch { /* ignore */ } finally { loading.value = false; }
};

// 新增用户
const handleAdd = () => {
  form.value = { isActive: true, roles: [] };
  dialogTitle.value = t('company.newUser');
  dialogVisible.value = true;
};

// 编辑用户
const handleEdit = (user: User) => {
  form.value = { ...user };
  dialogTitle.value = t('company.editUser');
  dialogVisible.value = true;
};

// 删除用户
const handleDelete = async (user: User) => {
  await ElMessageBox.confirm(t('expense.confirmDelete', { name: user.displayName }), t('common.confirm'), { type: 'warning' });
  try {
    await settingsApi.deleteUser(user.id);
    ElMessage.success(t('common.success'));
    fetchData();
  } catch { /* ignore */ }
};

// 重置密码
const handleResetPassword = async (user: User) => {
  await ElMessageBox.confirm(t('company.confirmResetPassword'), t('common.confirm'), { type: 'warning' });
  try {
    await settingsApi.resetUserPassword(user.id);
    ElMessage.success(t('company.passwordResetSuccess'));
  } catch { /* ignore */ }
};

// 保存
const handleSave = async () => {
  if (!form.value.username || !form.value.displayName) {
    ElMessage.warning(t('tax.fillRequired'));
    return;
  }
  saving.value = true;
  try {
    if (form.value.id) {
      await settingsApi.updateUser(form.value.id, form.value);
    } else {
      await settingsApi.createUser(form.value);
    }
    ElMessage.success(t('common.success'));
    dialogVisible.value = false;
    fetchData();
  } catch { /* ignore */ } finally { saving.value = false; }
};

// 分页变化
const handlePageChange = (page: number) => {
  pagination.value.page = page;
  fetchData();
};

onMounted(() => fetchData());
</script>

<template>
  <div class="users-view">
    <el-card>
      <template #header>
        <div class="card-header">
          <span class="card-title">{{ t('company.userManagement') }}</span>
          <div class="header-actions">
            <el-input v-model="searchKeyword" :placeholder="t('common.search')" :prefix-icon="Search" clearable style="width: 180px" />
            <el-select v-model="roleFilter" :placeholder="t('company.roles')" clearable style="width: 140px">
              <el-option v-for="r in roles" :key="r.id" :label="r.name" :value="r.code" />
            </el-select>
            <el-button :icon="Refresh" @click="fetchData">{{ t('common.reset') }}</el-button>
            <el-button type="primary" :icon="Plus" @click="handleAdd">{{ t('company.addUser') }}</el-button>
          </div>
        </div>
      </template>
      
      <el-table v-loading="loading" :data="filteredUsers" stripe>
        <el-table-column prop="username" :label="t('company.username')" width="120" />
        <el-table-column prop="displayName" :label="t('company.displayName')" width="120" />
        <el-table-column prop="email" :label="t('company.email')" min-width="180" />
        <el-table-column prop="phone" :label="t('company.phone')" width="130" />
        <el-table-column prop="roles" :label="t('company.roles')" min-width="150">
          <template #default="{ row }">
            <el-tag v-for="role in row.roles" :key="role" size="small" style="margin-right: 4px">
              {{ roles.find(r => r.code === role)?.name || role }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="isActive" :label="t('common.status')" width="80">
          <template #default="{ row }">
            <el-tag :type="row.isActive ? 'success' : 'danger'" size="small">
              {{ row.isActive ? t('settings.enabled') : t('settings.disabled') }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="lastLoginTime" :label="t('company.lastLogin')" width="160">
          <template #default="{ row }">
            {{ row.lastLoginTime ? new Date(row.lastLoginTime).toLocaleString() : '-' }}
          </template>
        </el-table-column>
        <el-table-column :label="t('common.actions')" width="160" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" size="small" @click="handleResetPassword(row)">
              <el-icon><Key /></el-icon>
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
      
      <div class="pagination-wrapper">
        <el-pagination v-model:current-page="pagination.page" :page-size="pagination.pageSize" :total="pagination.total" layout="total, prev, pager, next" @current-change="handlePageChange" />
      </div>
    </el-card>
    
    <!-- 编辑对话框 -->
    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="550px">
      <el-form :model="form" label-width="80px">
        <el-form-item :label="t('company.username')" required>
          <el-input v-model="form.username" :placeholder="t('company.enterUsername')" :disabled="!!form.id" />
        </el-form-item>
        <el-form-item :label="t('company.displayName')" required>
          <el-input v-model="form.displayName" :placeholder="t('company.enterDisplayName')" />
        </el-form-item>
        <el-form-item :label="t('company.email')">
          <el-input v-model="form.email" :placeholder="t('company.enterEmail')" />
        </el-form-item>
        <el-form-item :label="t('company.phone')">
          <el-input v-model="form.phone" :placeholder="t('company.enterPhone')" />
        </el-form-item>
        <el-form-item :label="t('company.roles')">
          <el-select v-model="form.roles" multiple :placeholder="t('company.selectRoles')" style="width: 100%">
            <el-option v-for="r in roles" :key="r.id" :label="r.name" :value="r.code" />
          </el-select>
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
.users-view {
  .card-header {
    display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; gap: 12px;
    .card-title { font-size: 16px; font-weight: 600; }
    .header-actions { display: flex; gap: 12px; flex-wrap: wrap; }
  }
  .pagination-wrapper { margin-top: 16px; display: flex; justify-content: flex-end; }
}
</style>
