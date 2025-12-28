// Hrevolve.Web 全局 using
// Web 层 - API 控制器、中间件

global using System.Net;
global using System.Security.Claims;
global using System.Text;
global using System.Text.Json;

// ASP.NET Core
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Filters;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Server.Kestrel.Core;
global using Microsoft.IdentityModel.Tokens;

// MediatR
global using MediatR;

// Serilog
global using Serilog;

// Scalar
global using Scalar.AspNetCore;

// 各层引用
global using Hrevolve.Agent;
global using Hrevolve.Agent.Services;
global using Hrevolve.Application;
global using Hrevolve.Infrastructure;
global using Hrevolve.Domain.Identity;
global using Hrevolve.Domain.Attendance;

// Application 层命令和查询
global using Hrevolve.Application.Employees.Commands;
global using Hrevolve.Application.Employees.Queries;
global using Hrevolve.Application.Identity.Commands;
global using Hrevolve.Application.Leave.Commands;

// 共享层
global using Hrevolve.Shared.Identity;
global using Hrevolve.Shared.MultiTenancy;
global using Hrevolve.Shared.Exceptions;

// Web 层内部
global using Hrevolve.Web.Middleware;
global using Hrevolve.Web.Filters;
