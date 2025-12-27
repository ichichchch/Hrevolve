using Hrevolve.Shared.Exceptions;
using Hrevolve.Shared.MultiTenancy;

namespace Hrevolve.Web.Middleware;

/// <summary>
/// 多租户中间件 - 解析并设置租户上下文
/// </summary>
public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantMiddleware> _logger;
    
    // 不需要租户的路径
    private static readonly string[] ExcludedPaths = 
    [
        "/health",
        "/swagger",
        "/api/auth/login",
        "/api/auth/register"
    ];
    
    public TenantMiddleware(RequestDelegate next, ILogger<TenantMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(
        HttpContext context,
        ITenantContextAccessor tenantContextAccessor,
        ITenantResolver tenantResolver)
    {
        // 检查是否为排除路径
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
        if (ExcludedPaths.Any(p => path.StartsWith(p)))
        {
            await _next(context);
            return;
        }
        
        // 尝试从多个来源解析租户
        var tenantIdentifier = ResolveTenantIdentifier(context);
        
        if (string.IsNullOrEmpty(tenantIdentifier))
        {
            // 如果是已认证用户，从JWT中获取租户ID
            var tenantIdClaim = context.User.FindFirst("tenant_id")?.Value;
            if (!string.IsNullOrEmpty(tenantIdClaim) && Guid.TryParse(tenantIdClaim, out var tenantId))
            {
                var tenantInfo = await tenantResolver.GetByIdAsync(tenantId);
                if (tenantInfo != null && tenantInfo.IsActive)
                {
                    tenantContextAccessor.TenantContext = new TenantContext(tenantInfo.Id, tenantInfo.Code);
                    await _next(context);
                    return;
                }
            }
            
            throw new TenantException("无法识别租户信息");
        }
        
        // 解析租户
        var tenant = await tenantResolver.ResolveAsync(tenantIdentifier);
        
        if (tenant == null)
        {
            throw new TenantException($"租户 '{tenantIdentifier}' 不存在");
        }
        
        if (!tenant.IsActive)
        {
            throw new TenantException("租户已被禁用");
        }
        
        // 设置租户上下文
        tenantContextAccessor.TenantContext = new TenantContext(tenant.Id, tenant.Code);
        
        _logger.LogDebug("租户上下文已设置: {TenantId} ({TenantCode})", tenant.Id, tenant.Code);
        
        try
        {
            await _next(context);
        }
        finally
        {
            // 清理租户上下文
            tenantContextAccessor.TenantContext = null;
        }
    }
    
    /// <summary>
    /// 从请求中解析租户标识
    /// 优先级: Header > Subdomain > Query
    /// </summary>
    private static string? ResolveTenantIdentifier(HttpContext context)
    {
        // 1. 从Header获取 (X-Tenant-Id)
        if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var headerValue))
        {
            return headerValue.ToString();
        }
        
        // 2. 从子域名获取 (tenant.example.com)
        var host = context.Request.Host.Host;
        var parts = host.Split('.');
        if (parts.Length >= 3)
        {
            return parts[0];
        }
        
        // 3. 从Query参数获取
        if (context.Request.Query.TryGetValue("tenant", out var queryValue))
        {
            return queryValue.ToString();
        }
        
        return null;
    }
}
