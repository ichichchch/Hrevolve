<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Plus, Edit, Delete, Search, Refresh, Key } from '@element-plus/icons-vue';
import { settingsApi } from '@/api';
import type { User, Role } from '@/types';

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
  dialogTitle.value = '新增用户';
  dialogVisible.value = true;
};

// 编辑用户
const handleEdit = (user: User) => {
  form.value = { ...user };
  dialogTitle.value = '编辑用户';
  dialogVisible.value = true;
};

// 删除用户
const handleDelete = async (user: User) => {
  await ElMessageBox.confirm(`确定删除用户"${user.displayName}"吗？`, '提示', { type: 'warning' });
  try {
    await settingsApi.deleteUser(user.id);
    ElMessage.success('删除成功');
    fetchData();
  } catch { /* ignore */ }
};

// 重置密码
const handleResetPassword = async (user: User) => {
  await ElMessageBox.confirm(`确定重置用户"${user.displayName}"的密码吗？`, '提示', { type: 'warning' });
  try {
    await settingsApi.resetUserPassword(user.id);
    ElMessage.success('密码已重置，新密码已发送至用户邮箱');
  } catch { /* ignore */ }
};

// 保存
const handleSave = async () => {
  if (!form.value.username || !form.value.displayName) {
    ElMessage.warning('请填写必填项');
    return;
  }
  saving.value = true;
  try {
    if (form.value.id) {
      await settingsApi.updateUser(form.value.id, form.value);
    } else {
      await settingsApi.createUser(form.value);
    }
    ElMessage.success('保存成功');
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
          <span class="card-title">用户管理</span>
          <div class="header-actions">
            <el-input v-model="searchKeyword" placeholder="搜索用户" :prefix-icon="Search" clearable style="width: 180px" />
            <el-select v-model="roleFilter" placeholder="角色筛选" clearable style="width: 140px">
              <el-option v-for="r in roles" :key="r.id" :label="r.name" :value="r.code" />
            </el-select>
            <el-button :icon="Refresh" @click="fetchData">刷新</el-button>
            <el-button type="primary" :icon="Plus" @click="handleAdd">新增用户</el-button>
          </div>
        </div>
      </template>
      
      <el-table v-loading="loading" :data="filteredUsers" stripe>
        <el-table-column prop="username" label="用户名" width="120" />
        <el-table-column prop="displayName" label="姓名" width="120" />
        <el-table-column prop="email" label="邮箱" min-width="180" />
        <el-table-column prop="phone" label="手机" width="130" />
        <el-table-column prop="roles" label="角色" min-width="150">
          <template #default="{ row }">
            <el-tag v-for="role in row.roles" :key="role" size="small" style="margin-right: 4px">
              {{ roles.find(r => r.code === role)?.name || role }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="isActive" label="状态" width="80">
          <template #default="{ row }">
            <el-tag :type="row.isActive ? 'success' : 'danger'" size="small">
              {{ row.isActive ? '启用' : '禁用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="lastLoginTime" label="最后登录" width="160">
          <template #default="{ row }">
            {{ row.lastLoginTime ? new Date(row.lastLoginTime).toLocaleString() : '-' }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="160" fixed="right">
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
        <el-pagination
          v-model:current-page="pagination.page"
          :page-size="pagination.pageSize"
          :total="pagination.total"
          layout="total, prev, pager, next"
          @current-change="handlePageChange"
        />
      </div>
    </el-card>
    
    <!-- 编辑对话框 -->
    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="550px">
      <el-form :model="form" label-width="80px">
        <el-form-item label="用户名" required>
          <el-input v-model="form.username" placeholder="请输入用户名" :disabled="!!form.id" />
        </el-form-item>
        <el-form-item label="姓名" required>
          <el-input v-model="form.displayName" placeholder="请输入姓名" />
        </el-form-item>
        <el-form-item label="邮箱">
          <el-input v-model="form.email" placeholder="请输入邮箱" />
        </el-form-item>
        <el-form-item label="手机">
          <el-input v-model="form.phone" placeholder="请输入手机号" />
        </el-form-item>
        <el-form-item label="角色">
          <el-select v-model="form.roles" multiple placeholder="请选择角色" style="width: 100%">
            <el-option v-for="r in roles" :key="r.id" :label="r.name" :value="r.code" />
          </el-select>
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
.users-view {
  .card-header {
    display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; gap: 12px;
    .card-title { font-size: 16px; font-weight: 600; }
    .header-actions { display: flex; gap: 12px; flex-wrap: wrap; }
  }
  .pagination-wrapper { margin-top: 16px; display: flex; justify-content: flex-end; }
}
</style>
