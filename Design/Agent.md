# HRM SaaS Project

---

## I. Codebase: Declare the Basics

This project is a modern, enterprise-grade SaaS Human Resource Management (HRM) system built with a focus on scalability, security, and extensibility.

- **Language**: C# 14
- **Backend Framework**: .NET 10 Web API  
- **Frontend Framework**: Vue 3 with Vite  
- **Test Framework**: xUnit  
- **AI Framework**: Microsoft Agent Framework + Microsoft.Extensions.AI
- **Project Architecture**: **Modular monolith** with clean boundaries; each core capability (e.g., Authentication, Organization, Payroll) is implemented as a distinct **"Agent Skill" module**, exposing well-defined internal/external APIs and adhering to the **Domain-Driven Design (DDD)** principles.

---

## II. Dependencies: List What Matters

Key NuGet packages for the .NET backend:

| Package | Purpose |
|--------|--------|
| `Microsoft.AspNetCore.Authentication.JwtBearer` | Validates JWT tokens to secure API endpoints |
| `Microsoft.AspNetCore.Authentication.Google` | Enables Google SSO integration |
| `Microsoft.AspNetCore.Authentication.OpenIdConnect` | Supports generic OIDC providers (e.g., Microsoft Entra ID) |
| `Microsoft.EntityFrameworkCore.Design` | Enables EF Core CLI tooling for migrations |
| `Npgsql.EntityFrameworkCore.PostgreSQL` | PostgreSQL provider for Entity Framework Core |
| `Serilog.AspNetCore` | Structured logging with Serilog integration |
| `Quartz.NET` | Job scheduling for recurring background tasks (e.g., payroll runs, leave accrual) |
| `Swashbuckle.AspNetCore` | Auto-generates OpenAPI 3.0 / Swagger documentation |
| `NRules` | .NET-native rules engine for dynamic payroll logic |
| `Microsoft.Agents.AI` | Microsoft Agent Framework - 下一代AI Agent框架（预览版） |
| `Microsoft.Extensions.AI` | 统一AI抽象层，支持多种LLM提供商 |
| `Microsoft.Extensions.AI.OpenAI` | OpenAI/Azure OpenAI集成 |
| `Microsoft.Extensions.VectorData.Abstractions` | 向量数据库抽象层，用于RAG |

> **Note**: Microsoft Agent Framework 是 Semantic Kernel 和 AutoGen 的下一代统一框架，提供更强大的Agent编排和工作流能力。

---

## III. Config: Point to Your Config Files

Configuration follows standard .NET configuration patterns using `IConfiguration`.

- **Primary Config File**: `appsettings.json`  
- **Environment Overrides**:  
  - `appsettings.Development.json`  
  - `appsettings.Production.json`

### Key Configuration Variables

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=hrm_saas;Username=...;Password=...",
    "Redis": "localhost:6379"
  },
  "Jwt": {
    "Key": "your-256-bit-secret-key-here",
    "Issuer": "https://hrm.yourcompany.com",
    "Audience": "https://hrm.yourcompany.com"
  },
  "AI": {
    "Provider": "OpenAI",
    "ApiKey": "sk-...",
    "Model": "gpt-4o",
    "Endpoint": "",
    "DeploymentName": ""
  },
  "GoogleAuth": {
    "ClientId": "...",
    "ClientSecret": "..."
  }
}
```

All secrets **must** be managed via environment variables or a secrets manager (e.g., Azure Key Vault, HashiCorp Vault) in production.

---

## IV. Backing Services: Declare External Services/Resources

### Database
- **Main Database**: PostgreSQL 16  
- **ORM**: Entity Framework Core 10  
- **Migration Strategy**: Code-first with **EF Core Migrations** (`dotnet ef migrations add`)

### Cache
- **Service**: Redis 7  
- **Use Cases**:  
  - Temporary storage for MFA codes  
  - SSO state and nonce caching  
  - Session tokens and tenant context  
  - Rate-limiting counters

### Message Queue
- **System**: RabbitMQ  
- **Use Cases**:  
  - Async payroll calculation triggers  
  - Bulk email/SMS notification dispatch  
  - Webhook event delivery with retry  
  - SCIM provisioning jobs

### Vector Database
- **Options**: ChromaDB (self-hosted) or Pinecone (cloud)  
- **Purpose**: Store vector embeddings of HR policy documents, FAQs, and onboarding guides  
- **Used By**: RAG-powered AI Assistant to retrieve relevant internal knowledge before LLM generation

---

## V. Build & Run: Document Your Workflow

Standard .NET CLI workflow for development and deployment.

### Commands

```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run in development (uses Kestrel, reads appsettings.Development.json)
dotnet run --project Hrevolve.Web

# Run in production (after publish)
dotnet Hrevolve.Web.dll
```

> Frontend (Vue 3 + Vite) is developed and served separately during development but is built into static assets and served by the .NET backend in production.

---

## VI. Port Binding: Specify Listening Port

The ASP.NET Core application listens on standard development ports:

| Protocol | Port | Purpose |
|--------|------|--------|
| HTTPS | `5001` | Secure traffic (development with self-signed cert) |
| HTTP  | `5000` | Insecure traffic (redirects to HTTPS in production) |

In production, the app is typically reverse-proxied behind **Nginx**, **Traefik**, or a cloud load balancer (e.g., AWS ALB), and only serves HTTPS on port 443.

---

## VII. Logs: Define Logging Practices

### Framework
- Primary: `Microsoft.Extensions.Logging`  
- Provider: **Serilog** with JSON formatting

### Logging Conventions

- ✅ **Use structured logging** in production (JSON output)
- ✅ Inject `ILogger<T>` via DI — **never** use `Console.WriteLine()`
- ✅ All log entries **must** include:
  - Timestamp (`UtcNow`)
  - Log level (`Information`, `Warning`, `Error`, etc.)
  - Message template with named parameters
  - Contextual properties:
    - `TraceId` (from distributed tracing)
    - `UserId`
    - `TenantId`
    - `CorrelationId`

### Example

```csharp
_logger.LogInformation(
    "User {@UserId} from tenant {@TenantId} initiated payroll run for period {@Period}",
    userId, tenantId, payrollPeriod);
```

### Output Targets
- Console (development)
- File / STDOUT (containerized environments)
- Centralized system: **Seq**, **ELK Stack**, **Datadog**, or **Azure Monitor**

> Logs are **never** used for business logic or auditing—audit trails are persisted separately in the database.

---

## VIII. AI Agent Architecture

### Microsoft Agent Framework Integration

The HR AI Assistant is built using **Microsoft Agent Framework**, the next-generation unified framework combining Semantic Kernel and AutoGen capabilities.

### Key Components

```
Hrevolve.Agent/
├── Services/
│   ├── HrAgentService.cs      # 主Agent服务，处理对话
│   └── HrToolProvider.cs      # 工具函数提供者
└── DependencyInjection.cs     # DI配置
```

### AI Tools (Functions)

| Tool | Description |
|------|-------------|
| `get_leave_balance` | 查询员工假期余额 |
| `submit_leave_request` | 提交请假申请 |
| `get_salary_info` | 查询薪资信息 |
| `get_attendance_records` | 查询考勤记录 |
| `query_hr_policy` | 查询HR政策（RAG） |
| `get_organization_info` | 查询组织架构 |
| `get_today_attendance` | 获取今日考勤状态 |

### Supported AI Providers

- **OpenAI** (GPT-4o, GPT-4-turbo)
- **Azure OpenAI**
- **Mock** (开发测试用)

### Configuration

```json
{
  "AI": {
    "Provider": "OpenAI",  // OpenAI | Azure | Mock
    "ApiKey": "sk-...",
    "Model": "gpt-4o",
    "Endpoint": "",        // Azure OpenAI endpoint
    "DeploymentName": ""   // Azure deployment name
  }
}
```

---
