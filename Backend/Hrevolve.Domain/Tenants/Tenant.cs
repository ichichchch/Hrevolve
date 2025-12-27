using Hrevolve.Domain.Common;

namespace Hrevolve.Domain.Tenants;

/// <summary>
/// 租户实体 - SaaS多租户核心
/// </summary>
public class Tenant : Entity
{
    public string Name { get; private set; } = null!;
    public string Code { get; private set; } = null!;
    public string? Domain { get; private set; }
    public TenantStatus Status { get; private set; }
    public TenantPlan Plan { get; private set; }
    public string? ConnectionString { get; private set; } // 独立数据库租户使用
    public string? EncryptionKey { get; private set; } // Per-Tenant加密密钥
    public TenantSettings Settings { get; private set; } = new();
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; private set; }
    
    private Tenant() { }
    
    public static Tenant Create(string name, string code, TenantPlan plan)
    {
        return new Tenant
        {
            Name = name,
            Code = code.ToLowerInvariant(),
            Plan = plan,
            Status = TenantStatus.Active
        };
    }
    
    public void SetDomain(string domain) => Domain = domain;
    public void SetConnectionString(string connectionString) => ConnectionString = connectionString;
    public void Suspend() => Status = TenantStatus.Suspended;
    public void Activate() => Status = TenantStatus.Active;
}

public enum TenantStatus
{
    Active,
    Suspended,
    Expired
}

public enum TenantPlan
{
    Free,
    Standard,
    Professional,
    Enterprise
}

/// <summary>
/// 租户配置
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
