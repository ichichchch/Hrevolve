using Hrevolve.Domain.Tenants;
using Hrevolve.Infrastructure.Persistence;
using Hrevolve.Shared.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Hrevolve.Infrastructure.MultiTenancy;

/// <summary>
/// 租户解析器实现
/// </summary>
public class TenantResolver : ITenantResolver
{
    private readonly HrevolveDbContext _context;
    private readonly IDistributedCache _cache;
    private const string CacheKeyPrefix = "tenant:";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);
    
    public TenantResolver(HrevolveDbContext context, IDistributedCache cache)
    {
        _context = context;
        _cache = cache;
    }
    
    /// <summary>
    /// 根据标识（域名或代码）解析租户
    /// </summary>
    public async Task<TenantInfo?> ResolveAsync(string identifier)
    {
        // 先从缓存获取
        var cacheKey = $"{CacheKeyPrefix}identifier:{identifier}";
        var cached = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cached))
        {
            return JsonSerializer.Deserialize<TenantInfo>(cached);
        }
        
        // 从数据库查询（忽略租户过滤器）
        var tenant = await _context.Tenants
            .IgnoreQueryFilters()
            .Where(t => t.Code == identifier || t.Domain == identifier)
            .FirstOrDefaultAsync();
        
        if (tenant == null) return null;
        
        var tenantInfo = MapToTenantInfo(tenant);
        
        // 缓存结果
        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(tenantInfo),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = CacheDuration });
        
        return tenantInfo;
    }
    
    /// <summary>
    /// 根据ID获取租户
    /// </summary>
    public async Task<TenantInfo?> GetByIdAsync(Guid tenantId)
    {
        var cacheKey = $"{CacheKeyPrefix}id:{tenantId}";
        var cached = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cached))
        {
            return JsonSerializer.Deserialize<TenantInfo>(cached);
        }
        
        var tenant = await _context.Tenants
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == tenantId);
        
        if (tenant == null) return null;
        
        var tenantInfo = MapToTenantInfo(tenant);
        
        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(tenantInfo),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = CacheDuration });
        
        return tenantInfo;
    }
    
    private static TenantInfo MapToTenantInfo(Tenant tenant)
    {
        return new TenantInfo
        {
            Id = tenant.Id,
            Name = tenant.Name,
            Code = tenant.Code,
            Domain = tenant.Domain,
            ConnectionString = tenant.ConnectionString,
            IsActive = tenant.Status == TenantStatus.Active,
            Settings = new Shared.MultiTenancy.TenantSettings
            {
                Timezone = tenant.Settings.Timezone,
                Locale = tenant.Settings.Locale,
                Currency = tenant.Settings.Currency,
                MaxEmployees = tenant.Settings.MaxEmployees,
                EnableMfa = tenant.Settings.EnableMfa,
                EnableSso = tenant.Settings.EnableSso
            }
        };
    }
}
