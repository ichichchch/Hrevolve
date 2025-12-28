# API接口文档

<cite>
**本文档引用的文件**  
- [Program.cs](file://Backend/Hrevolve.Web/Program.cs)
- [AuthController.cs](file://Backend/Hrevolve.Web/Controllers/AuthController.cs)
- [LoginCommand.cs](file://Backend/Hrevolve.Application/Identity/Commands/LoginCommand.cs)
- [CreateEmployeeCommand.cs](file://Backend/Hrevolve.Application/Employees/Commands/CreateEmployeeCommand.cs)
- [CreateLeaveRequestCommand.cs](file://Backend/Hrevolve.Application/Leave/Commands/CreateLeaveRequestCommand.cs)
- [EmployeesController.cs](file://Backend/Hrevolve.Web/Controllers/EmployeesController.cs)
- [LeaveController.cs](file://Backend/Hrevolve.Web/Controllers/LeaveController.cs)
- [OrganizationsController.cs](file://Backend/Hrevolve.Web/Controllers/OrganizationsController.cs)
- [AttendanceController.cs](file://Backend/Hrevolve.Web/Controllers/AttendanceController.cs)
- [PayrollController.cs](file://Backend/Hrevolve.Web/Controllers/PayrollController.cs)
- [ExpensesController.cs](file://Backend/Hrevolve.Web/Controllers/ExpensesController.cs)
- [SchedulesController.cs](file://Backend/Hrevolve.Web/Controllers/SchedulesController.cs)
- [InsuranceController.cs](file://Backend/Hrevolve.Web/Controllers/InsuranceController.cs)
- [TaxController.cs](file://Backend/Hrevolve.Web/Controllers/TaxController.cs)
- [RequirePermissionAttribute.cs](file://Backend/Hrevolve.Web/Filters/RequirePermissionAttribute.cs)
- [appsettings.json](file://Backend/Hrevolve.Web/appsettings.json)
- [appsettings.Development.json](file://Backend/Hrevolve.Web/appsettings.Development.json)
</cite>

## 目录
1. [简介](#简介)
2. [API版本控制与基础信息](#api版本控制与基础信息)
3. [认证与授权机制](#认证与授权机制)
4. [功能分组API端点](#功能分组api端点)
   - [认证相关接口](#认证相关接口)
   - [员工管理接口](#员工管理接口)
   - [组织架构接口](#组织架构接口)
   - [考勤管理接口](#考勤管理接口)
   - [假期管理接口](#假期管理接口)
   - [薪酬管理接口](#薪酬管理接口)
   - [报销管理接口](#报销管理接口)
   - [排班管理接口](#排班管理接口)
   - [保险福利接口](#保险福利接口)
   - [报税管理接口](#报税管理接口)
5. [请求/响应示例](#请求响应示例)
6. [速率限制与CORS配置](#速率限制与cors配置)
7. [Swagger/OpenAPI文档生成](#swaggeropenapi文档生成)

## 简介
Hrevolve HRM API 是一个企业级SaaS人力资源管理系统，提供全面的RESTful接口用于员工管理、考勤、假期、薪酬等核心HR功能。本API文档详细说明了所有公开端点的使用方法、参数要求、响应格式和错误码。

**本文档引用的文件**
- [Program.cs](file://Backend/Hrevolve.Web/Program.cs#L1-L173)
- [AuthController.cs](file://Backend/Hrevolve.Web/Controllers/AuthController.cs#L1-L89)

## API版本控制与基础信息
- **基础URL**: `https://api.hrevolve.local/v1`
- **API版本**: v1（通过URL路径和Swagger文档标识）
- **内容类型**: 所有请求和响应均使用 `application/json` 格式
- **日期格式**: `YYYY-MM-DD` (DateOnly) 和 `YYYY-MM-DDTHH:MM:SS` (DateTime)
- **HTTP/3支持**: 服务器配置支持HTTP/3协议，通过Alt-Svc头通知客户端

**本文档引用的文件**
- [Program.cs](file://Backend/Hrevolve.Web/Program.cs#L1-L173)

## 认证与授权机制
### JWT认证机制
系统采用JWT（JSON Web Token）进行身份验证，主要应用于 `/api/auth/*` 端点。

**认证流程**:
1. 用户通过 `/api/auth/login` 端点使用用户名密码登录
2. 服务器验证凭证后生成JWT Token
3. 客户端在后续请求的 `Authorization` 头中携带 `Bearer {token}`

**Token组成**:
- **Header**: 包含算法和Token类型
- **Payload**: 包含用户声明（Claims）
  - `sub`: 用户ID
  - `email`: 用户邮箱
  - `tenant_id`: 租户ID（多租户支持）
  - `username`: 用户名
  - `employee_id`: 员工ID（如关联）
  - `permission`: 用户权限列表
- **Signature**: 使用HMAC SHA-256算法签名

**配置参数**:
```json
{
  "Jwt": {
    "Key": "your-256-bit-secret-key-here-must-be-at-least-32-characters",
    "Issuer": "https://hrevolve.local",
    "Audience": "https://hrevolve.local"
  }
}
```

### 权限控制机制
系统使用自定义的 `RequirePermissionAttribute` 特性实现细粒度的权限控制。

**权限验证流程**:
1. 检查请求是否包含有效JWT Token
2. 从Token中提取用户权限列表
3. 验证用户是否具有执行操作所需的权限
4. 系统管理员（拥有 `system:admin` 权限）自动获得所有权限

**特性参数**:
- `params string[] permissions`: 所需权限列表
- `bool requireAll`: 是否需要同时拥有所有指定权限

**权限示例**:
- `employee:read` - 查看员工信息
- `employee:write` - 编辑员工信息
- `leave:approve` - 审批请假申请

```mermaid
sequenceDiagram
participant Client as "客户端"
participant AuthController as "认证控制器"
participant UserService as "用户服务"
participant TokenService as "令牌服务"
Client->>AuthController : POST /api/auth/login
AuthController->>UserService : 验证用户名密码
UserService-->>AuthController : 用户信息
AuthController->>TokenService : 生成JWT Token
TokenService-->>AuthController : 签名的JWT
AuthController-->>Client : {accessToken, refreshToken, ...}
Client->>EmployeesController : GET /api/employees/{id}
EmployeesController->>RequirePermission : 验证权限
RequirePermission->>CurrentUser : 获取当前用户
CurrentUser-->>RequirePermission : 权限列表
RequirePermission-->>EmployeesController : 权限验证结果
EmployeesController-->>Client : 员工详情或403错误
```

**Diagram sources**
- [Program.cs](file://Backend/Hrevolve.Web/Program.cs#L70-L86)
- [AuthController.cs](file://Backend/Hrevolve.Web/Controllers/AuthController.cs#L20-L38)
- [LoginCommand.cs](file://Backend/Hrevolve.Application/Identity/Commands/LoginCommand.cs#L46-L169)
- [RequirePermissionAttribute.cs](file://Backend/Hrevolve.Web/Filters/RequirePermissionAttribute.cs#L33-L78)

**本文档引用的文件**
- [Program.cs](file://Backend/Hrevolve.Web/Program.cs#L70-L86)
- [AuthController.cs](file://Backend/Hrevolve.Web/Controllers/AuthController.cs#L20-L38)
- [LoginCommand.cs](file://Backend/Hrevolve.Application/Identity/Commands/LoginCommand.cs#L46-L169)
- [RequirePermissionAttribute.cs](file://Backend/Hrevolve.Web/Filters/RequirePermissionAttribute.cs#L1-L80)

## 功能分组API端点
### 认证相关接口
#### 用户名密码登录
- **HTTP方法**: `POST`
- **URL路径**: `/api/auth/login`
- **权限要求**: 无需认证（AllowAnonymous）
- **请求参数**: 无URL参数
- **请求体结构**:
```json
{
  "username": "string",
  "password": "string",
  "tenantCode": "string",
  "deviceId": "string"
}
```
- **响应格式**:
```json
{
  "accessToken": "string",
  "refreshToken": "string",
  "expiresAt": "datetime",
  "userId": "guid",
  "userName": "string",
  "requiresMfa": "boolean"
}
```
- **可能的错误码**:
  - `INVALID_CREDENTIALS`: 用户名或密码错误
  - `ACCOUNT_LOCKED`: 账户已被锁定
  - `ACCOUNT_DISABLED`: 账户已被禁用

#### 刷新Token
- **HTTP方法**: `POST`
- **URL路径**: `/api/auth/refresh`
- **权限要求**: 无需认证（AllowAnonymous）
- **请求体结构**:
```json
{
  "refreshToken": "string"
}
```

#### 获取当前用户信息
- **HTTP方法**: `GET`
- **URL路径**: `/api/auth/me`
- **权限要求**: 需要认证（Authorize）
- **响应格式**:
```json
{
  "userId": "guid",
  "tenantId": "guid",
  "username": "string",
  "email": "string",
  "employeeId": "guid",
  "permissions": ["string"]
}
```

**本文档引用的文件**
- [AuthController.cs](file://Backend/Hrevolve.Web/Controllers/AuthController.cs#L17-L85)
- [LoginCommand.cs](file://Backend/Hrevolve.Application/Identity/Commands/LoginCommand.cs#L16-L169)

### 员工管理接口
#### 创建员工
- **HTTP方法**: `POST`
- **URL路径**: `/api/employees`
- **权限要求**: `employee:write`
- **请求体结构** (`CreateEmployeeCommand`):
```json
{
  "employeeNumber": "string",
  "firstName": "string",
  "lastName": "string",
  "englishName": "string",
  "gender": "Male|Female|Other",
  "dateOfBirth": "YYYY-MM-DD",
  "hireDate": "YYYY-MM-DD",
  "employmentType": "FullTime|PartTime|Contract",
  "email": "string",
  "phone": "string",
  "directManagerId": "guid",
  "positionId": "guid",
  "departmentId": "guid",
  "baseSalary": "decimal"
}
```
- **响应格式**: `201 Created` 带有新员工ID
- **可能的错误码**:
  - `DUPLICATE_EMPLOYEE_NUMBER`: 员工编号已存在

#### 获取员工详情
- **HTTP方法**: `GET`
- **URL路径**: `/api/employees/{id}`
- **权限要求**: `employee:read`
- **路径参数**: `id` (员工GUID)
- **响应格式**: 员工详细信息对象

#### 更新员工信息
- **HTTP方法**: `PUT`
- **URL路径**: `/api/employees/{id}`
- **权限要求**: `employee:write`
- **请求体结构**:
```json
{
  "email": "string",
  "phone": "string",
  "address": "string",
  "directManagerId": "guid"
}
```

#### 员工离职
- **HTTP方法**: `POST`
- **URL路径**: `/api/employees/{id}/terminate`
- **权限要求**: `employee:write`
- **请求体结构**:
```json
{
  "terminationDate": "YYYY-MM-DD",
  "reason": "string"
}
```

**本文档引用的文件**
- [EmployeesController.cs](file://Backend/Hrevolve.Web/Controllers/EmployeesController.cs#L18-L112)
- [CreateEmployeeCommand.cs](file://Backend/Hrevolve.Application/Employees/Commands/CreateEmployeeCommand.cs#L6-L126)

### 组织架构接口
#### 获取组织架构树
- **HTTP方法**: `GET`
- **URL路径**: `/api/organizations/tree`
- **权限要求**: `organization:read`

#### 创建组织单元
- **HTTP方法**: `POST`
- **URL路径**: `/api/organizations`
- **权限要求**: `organization:write`
- **请求体结构**:
```json
{
  "name": "string",
  "code": "string",
  "type": "Department|Team|Project",
  "parentId": "guid",
  "description": "string"
}
```

#### 获取组织单元下的员工
- **HTTP方法**: `GET`
- **URL路径**: `/api/organizations/{id}/employees`
- **权限要求**: `organization:read`
- **查询参数**:
  - `includeSubUnits`: 是否包含子单元员工 (boolean)

**本文档引用的文件**
- [OrganizationsController.cs](file://Backend/Hrevolve.Web/Controllers/OrganizationsController.cs#L18-L104)

### 考勤管理接口
#### 签到
- **HTTP方法**: `POST`
- **URL路径**: `/api/attendance/check-in`
- **权限要求**: 无需特定权限
- **请求体结构**:
```json
{
  "method": "Location|Wifi|FaceRecognition|Manual",
  "location": "string",
  "wifiSsid": "string"
}
```

#### 签退
- **HTTP方法**: `POST`
- **URL路径**: `/api/attendance/check-out`
- **权限要求**: 无需特定权限
- **请求体结构**: 与签到相同

#### 补卡申请
- **HTTP方法**: `POST`
- **URL路径**: `/api/attendance/manual-check`
- **权限要求**: 无需特定权限
- **请求体结构**:
```json
{
  "date": "YYYY-MM-DD",
  "checkInTime": "datetime",
  "checkOutTime": "datetime",
  "reason": "string"
}
```

#### 审批补卡申请
- **HTTP方法**: `POST`
- **URL路径**: `/api/attendance/manual-check/{id}/approve`
- **权限要求**: `attendance:approve`

**本文档引用的文件**
- [AttendanceController.cs](file://Backend/Hrevolve.Web/Controllers/AttendanceController.cs#L18-L132)

### 假期管理接口
#### 提交请假申请
- **HTTP方法**: `POST`
- **URL路径**: `/api/leave/requests`
- **权限要求**: `leave:write`
- **请求体结构** (`CreateLeaveRequestCommand`):
```json
{
  "leaveTypeId": "guid",
  "startDate": "YYYY-MM-DD",
  "endDate": "YYYY-MM-DD",
  "startDayPart": "FullDay|Morning|Afternoon",
  "endDayPart": "FullDay|Morning|Afternoon",
  "reason": "string",
  "attachments": ["string"]
}
```
- **业务验证**:
  - 检查假期余额是否充足
  - 验证日期不与现有请假冲突
  - 验证假期类型是否有效

#### 审批请假申请
- **HTTP方法**: `POST`
- **URL路径**: `/api/leave/requests/{id}/approve`
- **权限要求**: `leave:approve`
- **请求体结构**:
```json
{
  "comments": "string"
}
```

#### 拒绝请假申请
- **HTTP方法**: `POST`
- **URL路径**: `/api/leave/requests/{id}/reject`
- **权限要求**: `leave:approve`
- **请求体结构**:
```json
{
  "reason": "string"
}
```

#### 获取我的假期余额
- **HTTP方法**: `GET`
- **URL路径**: `/api/leave/balances/my`
- **权限要求**: 无需特定权限
- **查询参数**: `year` (可选)

**本文档引用的文件**
- [LeaveController.cs](file://Backend/Hrevolve.Web/Controllers/LeaveController.cs#L19-L128)
- [CreateLeaveRequestCommand.cs](file://Backend/Hrevolve.Application/Leave/Commands/CreateLeaveRequestCommand.cs#L6-L149)

### 薪酬管理接口
#### 创建薪资周期
- **HTTP方法**: `POST`
- **URL路径**: `/api/payroll/periods`
- **权限要求**: `payroll:write`
- **请求体结构**:
```json
{
  "year": "int",
  "month": "int",
  "startDate": "YYYY-MM-DD",
  "endDate": "YYYY-MM-DD",
  "payDate": "YYYY-MM-DD"
}
```

#### 执行薪资计算
- **HTTP方法**: `POST`
- **URL路径**: `/api/payroll/periods/{periodId}/calculate`
- **权限要求**: `payroll:write`
- **查询参数**: `isDryRun` (是否为试算，默认true)

#### 锁定薪资周期
- **HTTP方法**: `POST`
- **URL路径**: `/api/payroll/periods/{periodId}/lock`
- **权限要求**: `payroll:approve`

#### 获取我的薪资单
- **HTTP方法**: `GET`
- **URL路径**: `/api/payroll/records/my`
- **权限要求**: 无需特定权限
- **查询参数**: `year` (可选)

**本文档引用的文件**
- [PayrollController.cs](file://Backend/Hrevolve.Web/Controllers/PayrollController.cs#L18-L137)

### 报销管理接口
#### 获取报销类型列表
- **HTTP方法**: `GET`
- **URL路径**: `/api/expenses/types`
- **权限要求**: 无需特定权限

#### 创建报销申请
- **HTTP方法**: `POST`
- **URL路径**: `/api/expenses/requests`
- **权限要求**: 无需特定权限

#### 审批报销申请
- **HTTP方法**: `POST`
- **URL路径**: `/api/expenses/requests/{id}/approve`
- **权限要求**: 无需特定权限

**本文档引用的文件**
- [ExpensesController.cs](file://Backend/Hrevolve.Web/Controllers/ExpensesController.cs#L12-L89)

### 排班管理接口
#### 获取排班列表
- **HTTP方法**: `GET`
- **URL路径**: `/api/schedules`
- **权限要求**: 无需特定权限
- **查询参数**: `startDate`, `endDate`

#### 分配排班
- **HTTP方法**: `POST`
- **URL路径**: `/api/schedules/assign`
- **权限要求**: 无需特定权限

#### 获取班次模板列表
- **HTTP方法**: `GET`
- **URL路径**: `/api/schedules/shift-templates`
- **权限要求**: 无需特定权限

**本文档引用的文件**
- [SchedulesController.cs](file://Backend/Hrevolve.Web/Controllers/SchedulesController.cs#L14-L97)

### 保险福利接口
#### 获取保险方案列表
- **HTTP方法**: `GET`
- **URL路径**: `/api/insurance/plans`
- **权限要求**: 无需特定权限

#### 员工参保
- **HTTP方法**: `POST`
- **URL路径**: `/api/insurance/enroll`
- **权限要求**: 无需特定权限

#### 获取福利项目列表
- **HTTP方法**: `GET`
- **URL路径**: `/api/insurance/benefits-simple`
- **权限要求**: 无需特定权限

**本文档引用的文件**
- [InsuranceController.cs](file://Backend/Hrevolve.Web/Controllers/InsuranceController.cs#L14-L139)

### 报税管理接口
#### 获取税务档案列表
- **HTTP方法**: `GET`
- **URL路径**: `/api/tax/profiles`
- **权限要求**: 无需特定权限

#### 获取报税记录列表
- **HTTP方法**: `GET`
- **URL路径**: `/api/tax/records`
- **权限要求**: 无需特定权限
- **查询参数**: `year`, `month` (可选)

#### 导出报税记录
- **HTTP方法**: `GET`
- **URL路径**: `/api/tax/records/export`
- **权限要求**: 无需特定权限

#### 获取税务设置
- **HTTP方法**: `GET`
- **URL路径**: `/api/tax/settings`
- **权限要求**: 无需特定权限

**本文档引用的文件**
- [TaxController.cs](file://Backend/Hrevolve.Web/Controllers/TaxController.cs#L14-L115)

## 请求/响应示例
### 创建员工请求示例
```json
POST /api/employees
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "employeeNumber": "EMP001",
  "firstName": "张",
  "lastName": "三",
  "gender": "Male",
  "dateOfBirth": "1990-01-01",
  "hireDate": "2024-01-01",
  "employmentType": "FullTime",
  "email": "zhangsan@company.com",
  "phone": "13800138000",
  "positionId": "a1b2c3d4-e5f6-7890-1234-567890abcdef",
  "departmentId": "f4e3d2c1-b0a9-8765-4321-fedcba098765",
  "baseSalary": 15000.00
}
```

### 创建员工响应示例
```json
HTTP/1.1 201 Created
Location: /api/employees/a1b2c3d4-e5f6-7890-1234-567890abcdef

{
  "id": "a1b2c3d4-e5f6-7890-1234-567890abcdef"
}
```

### 提交请假申请请求示例
```json
POST /api/leave/requests
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "leaveTypeId": "a1b2c3d4-e5f6-7890-1234-567890abcdef",
  "startDate": "2024-12-25",
  "endDate": "2024-12-27",
  "startDayPart": "FullDay",
  "endDayPart": "FullDay",
  "reason": "家庭聚会",
  "attachments": [
    "https://storage.hrevolve.local/attachments/123.pdf"
  ]
}
```

### 提交请假申请响应示例
```json
HTTP/1.1 201 Created
Location: /api/leave/requests/a1b2c3d4-e5f6-7890-1234-567890abcdef

{
  "id": "a1b2c3d4-e5f6-7890-1234-567890abcdef"
}
```

### 认证失败响应示例
```json
HTTP/1.1 401 Unauthorized

{
  "code": "UNAUTHORIZED",
  "message": "未授权访问"
}
```

### 权限不足响应示例
```json
HTTP/1.1 403 Forbidden

{
  "code": "FORBIDDEN",
  "message": "权限不足"
}
```

### 参数验证失败响应示例
```json
HTTP/1.1 400 Bad Request

{
  "code": "VALIDATION_ERROR",
  "message": "员工编号不能为空"
}
```

**本文档引用的文件**
- [CreateEmployeeCommand.cs](file://Backend/Hrevolve.Application/Employees/Commands/CreateEmployeeCommand.cs#L8-L24)
- [CreateLeaveRequestCommand.cs](file://Backend/Hrevolve.Application/Leave/Commands/CreateLeaveRequestCommand.cs#L8-L15)

## 速率限制与CORS配置
### 速率限制机制
系统通过中间件实现API速率限制，保护服务器资源：

- **限制策略**:
  - 每分钟最多100次请求（普通用户）
  - 每分钟最多1000次请求（管理员）
  - 每小时最多5000次请求（防暴力破解）

- **实现方式**:
  - 基于Redis的分布式计数器
  - 按IP地址和用户ID进行限制
  - 返回 `X-RateLimit-Limit` 和 `X-RateLimit-Remaining` 头信息

### CORS配置
系统配置了宽松的CORS策略以支持前端应用：

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

- **允许来源**: 所有 (`*`)
- **允许方法**: 所有HTTP方法
- **允许头部**: 所有头部
- **凭证**: 允许携带凭据

**本文档引用的文件**
- [Program.cs](file://Backend/Hrevolve.Web/Program.cs#L90-L98)

## Swagger/OpenAPI文档生成
系统集成Swagger UI来自动生成API文档，便于开发者测试和探索API。

### 配置详情
```csharp
// Swagger配置
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Hrevolve HRM API",
        Version = "v1",
        Description = "企业级SaaS人力资源管理系统API - 基于.NET 10和Microsoft Agent Framework"
    });
    
    // JWT认证配置
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
```

### 访问方式
- **开发环境**: `https://localhost:5225/swagger`
- **生产环境**: `https://api.hrevolve.local/v1/swagger`

### 功能特点
- 交互式API测试界面
- 自动生成的请求/响应示例
- 支持JWT认证的测试
- 实时查看API调用结果
- 支持导出OpenAPI规范文件

**本文档引用的文件**
- [Program.cs](file://Backend/Hrevolve.Web/Program.cs#L34-L68)