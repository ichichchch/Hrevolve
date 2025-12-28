// Hrevolve.Infrastructure 全局 using
// 基础设施层 - 数据访问、外部服务集成

global using System.Linq.Expressions;

// Microsoft 扩展
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Caching.Distributed;
global using Microsoft.Extensions.Logging;

// 领域层
global using Hrevolve.Domain.Common;
global using Hrevolve.Domain.Employees;
global using Hrevolve.Domain.Identity;
global using Hrevolve.Domain.Organizations;
global using Hrevolve.Domain.Leave;
global using Hrevolve.Domain.Attendance;
global using Hrevolve.Domain.Payroll;
global using Hrevolve.Domain.Tenants;

// 共享层
global using Hrevolve.Shared.MultiTenancy;
global using Hrevolve.Shared.Identity;
global using Hrevolve.Shared.Exceptions;

// 基础设施层内部
global using Hrevolve.Infrastructure.Persistence;
