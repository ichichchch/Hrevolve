// Hrevolve.Application 全局 using
// 应用层 - CQRS、业务逻辑编排

global using System.Diagnostics;
global using System.IdentityModel.Tokens.Jwt;
global using System.Reflection;
global using System.Security.Claims;
global using System.Text;

// MediatR
global using MediatR;

// FluentValidation
global using FluentValidation;

// Microsoft 扩展
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.IdentityModel.Tokens;

// 领域层
global using Hrevolve.Domain.Common;
global using Hrevolve.Domain.Employees;
global using Hrevolve.Domain.Identity;
global using Hrevolve.Domain.Organizations;
global using Hrevolve.Domain.Leave;
global using Hrevolve.Domain.Attendance;
global using Hrevolve.Domain.Payroll;

// 共享层
global using Hrevolve.Shared.MultiTenancy;
global using Hrevolve.Shared.Identity;
global using Hrevolve.Shared.Exceptions;
global using Hrevolve.Shared.Results;

// 基础设施层（仓储接口）
global using Hrevolve.Infrastructure.Persistence;
global using Hrevolve.Infrastructure.Persistence.Repositories;

// 应用层内部
global using Hrevolve.Application.Behaviors;
