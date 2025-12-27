import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router';
import { useAuthStore } from '@/stores/auth';

// 路由配置
const routes: RouteRecordRaw[] = [
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/views/auth/LoginView.vue'),
    meta: { requiresAuth: false, title: '登录' },
  },
  {
    path: '/',
    component: () => import('@/layouts/MainLayout.vue'),
    meta: { requiresAuth: true },
    children: [
      {
        path: '',
        name: 'Dashboard',
        component: () => import('@/views/dashboard/DashboardView.vue'),
        meta: { title: '工作台', icon: 'HomeFilled' },
      },
      // 员工自助
      {
        path: 'self-service',
        name: 'SelfService',
        redirect: '/self-service/profile',
        meta: { title: '员工自助', icon: 'User' },
        children: [
          {
            path: 'profile',
            name: 'MyProfile',
            component: () => import('@/views/self-service/ProfileView.vue'),
            meta: { title: '个人信息' },
          },
          {
            path: 'attendance',
            name: 'MyAttendance',
            component: () => import('@/views/self-service/AttendanceView.vue'),
            meta: { title: '我的考勤' },
          },
          {
            path: 'leave',
            name: 'MyLeave',
            component: () => import('@/views/self-service/LeaveView.vue'),
            meta: { title: '我的假期' },
          },
          {
            path: 'payroll',
            name: 'MyPayroll',
            component: () => import('@/views/self-service/PayrollView.vue'),
            meta: { title: '我的薪资' },
          },
        ],
      },
      // AI助手
      {
        path: 'assistant',
        name: 'Assistant',
        component: () => import('@/views/assistant/AssistantView.vue'),
        meta: { title: 'AI助手', icon: 'ChatDotRound' },
      },
      // 组织管理
      {
        path: 'organization',
        name: 'Organization',
        redirect: '/organization/structure',
        meta: { title: '组织管理', icon: 'OfficeBuilding', permission: 'organization:read' },
        children: [
          {
            path: 'structure',
            name: 'OrgStructure',
            component: () => import('@/views/organization/StructureView.vue'),
            meta: { title: '组织架构' },
          },
          {
            path: 'positions',
            name: 'Positions',
            component: () => import('@/views/organization/PositionsView.vue'),
            meta: { title: '职位管理' },
          },
        ],
      },
      // 员工管理
      {
        path: 'employees',
        name: 'Employees',
        redirect: '/employees',
        meta: { title: '员工管理', icon: 'UserFilled', permission: 'employee:read' },
        children: [
          {
            path: '',
            name: 'EmployeeList',
            component: () => import('@/views/employees/EmployeeListView.vue'),
            meta: { title: '员工列表' },
          },
          {
            path: ':id',
            name: 'EmployeeDetail',
            component: () => import('@/views/employees/EmployeeDetailView.vue'),
            meta: { title: '员工详情', hidden: true },
          },
        ],
      },
      // 考勤管理
      {
        path: 'attendance',
        name: 'Attendance',
        redirect: '/attendance/records',
        meta: { title: '考勤管理', icon: 'Clock', permission: 'attendance:read' },
        children: [
          {
            path: 'records',
            name: 'AttendanceRecords',
            component: () => import('@/views/attendance/RecordsView.vue'),
            meta: { title: '考勤记录' },
          },
          {
            path: 'shifts',
            name: 'Shifts',
            component: () => import('@/views/attendance/ShiftsView.vue'),
            meta: { title: '班次管理' },
          },
        ],
      },
      // 假期管理
      {
        path: 'leave',
        name: 'Leave',
        redirect: '/leave/requests',
        meta: { title: '假期管理', icon: 'Calendar', permission: 'leave:read' },
        children: [
          {
            path: 'requests',
            name: 'LeaveRequests',
            component: () => import('@/views/leave/RequestsView.vue'),
            meta: { title: '请假申请' },
          },
          {
            path: 'approvals',
            name: 'LeaveApprovals',
            component: () => import('@/views/leave/ApprovalsView.vue'),
            meta: { title: '假期审批' },
          },
          {
            path: 'types',
            name: 'LeaveTypes',
            component: () => import('@/views/leave/TypesView.vue'),
            meta: { title: '假期类型' },
          },
        ],
      },
      // 薪酬管理
      {
        path: 'payroll',
        name: 'Payroll',
        redirect: '/payroll/records',
        meta: { title: '薪酬管理', icon: 'Money', permission: 'payroll:read' },
        children: [
          {
            path: 'records',
            name: 'PayrollRecords',
            component: () => import('@/views/payroll/RecordsView.vue'),
            meta: { title: '薪资记录' },
          },
          {
            path: 'periods',
            name: 'PayrollPeriods',
            component: () => import('@/views/payroll/PeriodsView.vue'),
            meta: { title: '薪资周期' },
          },
        ],
      },
      // 公司设置
      {
        path: 'company',
        name: 'Company',
        redirect: '/company/tenant',
        meta: { title: '公司设置', icon: 'Setting', permission: 'settings:read' },
        children: [
          {
            path: 'tenant',
            name: 'CompanyTenant',
            component: () => import('@/views/company/TenantView.vue'),
            meta: { title: '公司信息' },
          },
          {
            path: 'cost-centers',
            name: 'CostCenters',
            component: () => import('@/views/company/CostCentersView.vue'),
            meta: { title: '成本中心' },
          },
          {
            path: 'tags',
            name: 'Tags',
            component: () => import('@/views/company/TagsView.vue'),
            meta: { title: '标签管理' },
          },
          {
            path: 'clock-devices',
            name: 'ClockDevices',
            component: () => import('@/views/company/ClockDevicesView.vue'),
            meta: { title: '打卡设备' },
          },
          {
            path: 'users',
            name: 'CompanyUsers',
            component: () => import('@/views/company/UsersView.vue'),
            meta: { title: '用户管理' },
          },
        ],
      },
      // 排班管理
      {
        path: 'schedule',
        name: 'Schedule',
        redirect: '/schedule/overview',
        meta: { title: '排班管理', icon: 'Calendar', permission: 'attendance:read' },
        children: [
          {
            path: 'overview',
            name: 'ScheduleOverview',
            component: () => import('@/views/schedule/OverviewView.vue'),
            meta: { title: '排班概览' },
          },
          {
            path: 'table',
            name: 'ScheduleTable',
            component: () => import('@/views/schedule/ScheduleTableView.vue'),
            meta: { title: '排班表' },
          },
          {
            path: 'templates',
            name: 'ShiftTemplates',
            component: () => import('@/views/schedule/ShiftTemplatesView.vue'),
            meta: { title: '班次模板' },
          },
          {
            path: 'calendar',
            name: 'ScheduleCalendar',
            component: () => import('@/views/schedule/CalendarView.vue'),
            meta: { title: '排班日历' },
          },
        ],
      },
      // 报销管理
      {
        path: 'expense',
        name: 'Expense',
        redirect: '/expense/requests',
        meta: { title: '报销管理', icon: 'Wallet', permission: 'expense:read' },
        children: [
          {
            path: 'requests',
            name: 'ExpenseRequests',
            component: () => import('@/views/expense/RequestsView.vue'),
            meta: { title: '报销申请' },
          },
          {
            path: 'types',
            name: 'ExpenseTypes',
            component: () => import('@/views/expense/TypesView.vue'),
            meta: { title: '报销类型' },
          },
        ],
      },
      // 保险福利
      {
        path: 'insurance',
        name: 'Insurance',
        redirect: '/insurance/overview',
        meta: { title: '保险福利', icon: 'FirstAidKit', permission: 'payroll:read' },
        children: [
          {
            path: 'overview',
            name: 'InsuranceOverview',
            component: () => import('@/views/insurance/OverviewView.vue'),
            meta: { title: '概览' },
          },
          {
            path: 'plans',
            name: 'InsurancePlans',
            component: () => import('@/views/insurance/PlansView.vue'),
            meta: { title: '保险方案' },
          },
          {
            path: 'employees',
            name: 'EmployeeInsurance',
            component: () => import('@/views/insurance/EmployeeInsuranceView.vue'),
            meta: { title: '员工参保' },
          },
          {
            path: 'benefits',
            name: 'Benefits',
            component: () => import('@/views/insurance/BenefitsView.vue'),
            meta: { title: '福利项目' },
          },
        ],
      },
      // 报税管理
      {
        path: 'tax',
        name: 'Tax',
        redirect: '/tax/profiles',
        meta: { title: '报税管理', icon: 'Document', permission: 'payroll:read' },
        children: [
          {
            path: 'profiles',
            name: 'TaxProfiles',
            component: () => import('@/views/tax/ProfilesView.vue'),
            meta: { title: '税务档案' },
          },
          {
            path: 'records',
            name: 'TaxRecords',
            component: () => import('@/views/tax/RecordsView.vue'),
            meta: { title: '报税记录' },
          },
          {
            path: 'settings',
            name: 'TaxSettings',
            component: () => import('@/views/tax/SettingsView.vue'),
            meta: { title: '税务设置' },
          },
        ],
      },
      // 系统设置
      {
        path: 'settings',
        name: 'Settings',
        redirect: '/settings/configs',
        meta: { title: '系统设置', icon: 'Tools', permission: 'settings:read' },
        children: [
          {
            path: 'configs',
            name: 'SystemConfigs',
            component: () => import('@/views/settings/ConfigsView.vue'),
            meta: { title: '系统配置' },
          },
          {
            path: 'roles',
            name: 'Roles',
            component: () => import('@/views/settings/RolesView.vue'),
            meta: { title: '角色管理' },
          },
          {
            path: 'approval-flows',
            name: 'ApprovalFlows',
            component: () => import('@/views/settings/ApprovalFlowsView.vue'),
            meta: { title: '审批流程' },
          },
          {
            path: 'audit-logs',
            name: 'AuditLogs',
            component: () => import('@/views/settings/AuditLogsView.vue'),
            meta: { title: '审计日志' },
          },
        ],
      },
    ],
  },
  // 404
  {
    path: '/:pathMatch(.*)*',
    name: 'NotFound',
    component: () => import('@/views/error/NotFoundView.vue'),
    meta: { title: '页面不存在' },
  },
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
});

// 路由守卫
router.beforeEach(async (to, _from, next) => {
  const authStore = useAuthStore();
  
  // 设置页面标题
  document.title = `${to.meta.title || 'Hrevolve'} - Hrevolve HRM`;
  
  // 不需要认证的页面
  if (to.meta.requiresAuth === false) {
    if (authStore.isAuthenticated && to.name === 'Login') {
      next({ name: 'Dashboard' });
    } else {
      next();
    }
    return;
  }
  
  // 需要认证的页面
  if (!authStore.isAuthenticated) {
    next({ name: 'Login', query: { redirect: to.fullPath } });
    return;
  }
  
  // 获取用户信息
  if (!authStore.user) {
    await authStore.fetchUser();
  }
  
  // 检查权限 - 开发环境跳过权限检查
  const requiredPermission = to.meta.permission as string | undefined;
  if (requiredPermission && !import.meta.env.DEV) {
    if (!authStore.hasPermission(requiredPermission)) {
      next({ name: 'Dashboard' });
      return;
    }
  }
  
  next();
});

export default router;
