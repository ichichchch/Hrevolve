# Hrevolve - 企业级SaaS人力资源管理系统

## 项目概述

Hrevolve是一个现代化的企业级SaaS人力资源管理系统，采用模块化单体架构（Modular Monolith）和领域驱动设计（DDD）原则构建。

## 技术栈

- **后端框架**: .NET 10 / ASP.NET Core Web API
- **AI框架**: Microsoft Agent Framework + Microsoft.Extensions.AI
- **数据库**: PostgreSQL 16
- **ORM**: Entity Framework Core 10
- **缓存**: Redis 7
- **认证**: JWT + OAuth 2.0 (SSO)
- **日志**: Serilog
- **API文档**: Swagger/OpenAPI

## 项目结构

```
Backend/
├── Hrevolve.Domain/          # 领域层 - 实体、值对象、领域事件
├── Hrevolve.Shared/          # 共享层 - 通用组件、多租户、异常
├── Hrevolve.Infrastructure/  # 基础设施层 - 数据访问、外部服务
├── Hrevolve.Application/     # 应用层 - 业务逻辑、CQRS命令/查询
├── Hrevolve.Agent/           # AI代理层 - Microsoft Agent Framework
└── Hrevolve.Web/             # Web层 - API控制器、中间件
```

## 核心功能模块

### 已实现
- ✅ 多租户架构（Query Rewriting + RLS）
- ✅ 组织架构管理（树状结构，邻接表+路径枚举）
- ✅ 员工全生命周期管理（SCD Type 2历史追溯）
- ✅ 用户认证（JWT、SSO支持）
- ✅ RBAC权限控制
- ✅ 考勤管理
- ✅ 假期管理（策略引擎）
- ✅ 薪酬管理
- ✅ 报销管理
- ✅ 审计日志
- ✅ AI助手（Microsoft Agent Framework）

### 待完善
- 🔲 薪酬计算规则引擎（NRules）
- 🔲 RAG知识库集成（向量数据库）
- 🔲 员工离职风险预测
- 🔲 Webhook事件推送
- 🔲 SCIM身份供给

## 快速开始

### 环境要求

- .NET 10 Preview SDK
- PostgreSQL 16+
- Redis 7+

### 配置

1. 修改 `appsettings.Development.json` 中的数据库连接字符串和AI配置

### 运行

```bash
# 还原依赖
dotnet restore

# 创建数据库迁移
dotnet ef migrations add InitialCreate -p Hrevolve.Infrastructure -s Hrevolve.Web

# 应用迁移
dotnet ef database update -p Hrevolve.Infrastructure -s Hrevolve.Web

# 运行应用
dotnet run --project Hrevolve.Web
```

### API文档

启动后访问: `https://localhost:5001/swagger`

## API端点

### 认证
- `POST /api/auth/login` - 用户登录
- `POST /api/auth/refresh` - 刷新Token
- `GET /api/auth/me` - 获取当前用户信息

### AI助手
- `POST /api/agent/chat` - 与HR助手对话
- `GET /api/agent/history` - 获取对话历史
- `DELETE /api/agent/history` - 清除对话历史

### 员工管理
- `GET /api/employees/{id}` - 获取员工详情
- `GET /api/employees/{id}/at-date?date=2024-01-01` - 历史时点查询
- `POST /api/employees` - 创建员工

### 组织架构
- `GET /api/organizations/tree` - 获取组织架构树
- `GET /api/organizations/{id}` - 获取组织单元详情

### 假期管理
- `POST /api/leave/requests` - 提交请假申请
- `GET /api/leave/balances/my` - 获取我的假期余额

### 考勤管理
- `POST /api/attendance/check-in` - 签到
- `POST /api/attendance/check-out` - 签退

### 薪酬管理
- `GET /api/payroll/records/my` - 获取我的薪资单

## AI助手配置

系统集成了Microsoft Agent Framework，支持多种AI提供商：

```json
{
  "AI": {
    "Provider": "OpenAI",  // OpenAI | Azure | Mock
    "ApiKey": "sk-...",
    "Model": "gpt-4o"
  }
}
```

### 支持的AI工具函数
- `get_leave_balance` - 查询假期余额
- `submit_leave_request` - 提交请假申请
- `get_salary_info` - 查询薪资信息
- `get_attendance_records` - 查询考勤记录
- `query_hr_policy` - 查询HR政策（RAG）
- `get_organization_info` - 查询组织架构

## 多租户

系统支持多种租户识别方式：
1. HTTP Header: `X-Tenant-Id`
2. 子域名: `{tenant}.example.com`
3. Query参数: `?tenant={code}`
4. JWT Token中的`tenant_id`声明

## 安全特性

- JWT Token认证
- RBAC权限控制
- 多租户数据隔离
- 敏感数据加密（Per-Tenant Key）
- 审计日志
- 软删除

## 许可证

MIT License
