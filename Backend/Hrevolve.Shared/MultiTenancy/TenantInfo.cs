namespace Hrevolve.Shared.MultiTenancy;

/// <summary>
/// 租户信息 - 用于缓存和传递租户配置
/// </summary>
public class TenantInfo
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string? Domain { get; set; }
    public string? ConnectionString { get; set; }
    public TenantSettings Settings { get; set; } = new();
    public bool IsActive { get; set; }
}

/// <summary>
/// 租户设置
/// </summary>
public class TenantSettings
{
    public string Timezone { get; set; } = "Asia/Shanghai";
    public string Locale { get; set; } = "zh-CN";
    public string Currency { get; set; } = "CNY";
    public int MaxEmployees { get; set; } = 100;
    public bool EnableMfa { get; set; } = true;
    public bool EnableSso { get; set; }
}

/// <summary>
/// 租户解析器接口
/// </summary>
public interface ITenantResolver
{
    /// <summary>
    /// 根据标识解析租户
    /// </summary>
    Task<TenantInfo?> ResolveAsync(string identifier);
    
    /// <summary>
    /// 根据ID获取租户
    /// </summary>
    Task<TenantInfo?> GetByIdAsync(Guid tenantId);
}
