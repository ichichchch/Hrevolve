namespace Hrevolve.Shared.MultiTenancy;

/// <summary>
/// 租户上下文接口 - 提供当前请求的租户信息
/// </summary>
public interface ITenantContext
{
    /// <summary>
    /// 当前租户ID
    /// </summary>
    Guid TenantId { get; }
    
    /// <summary>
    /// 租户代码
    /// </summary>
    string? TenantCode { get; }
    
    /// <summary>
    /// 是否已设置租户
    /// </summary>
    bool HasTenant { get; }
}


/// <summary>
/// 租户上下文访问器 - 用于设置和获取租户上下文
/// </summary>
public interface ITenantContextAccessor
{
    /// <summary>
    /// 获取当前租户上下文
    /// </summary>
    ITenantContext? TenantContext { get; set; }
}

/// <summary>
/// 租户上下文实现
/// </summary>
public class TenantContext(Guid tenantId, string? tenantCode = null) : ITenantContext
{
    public Guid TenantId { get; } = tenantId;

    public string? TenantCode { get; } = tenantCode;

    public bool HasTenant => TenantId != Guid.Empty;
}

/// <summary>
/// 租户上下文访问器实现 - 使用AsyncLocal存储
/// </summary>
public class TenantContextAccessor : ITenantContextAccessor
{

    private static readonly AsyncLocal<TenantContextHolder> _tenantContextCurrent = new();

    public ITenantContext? TenantContext
    {
        get => _tenantContextCurrent.Value?.Context;
        set
        {
            if (value == null)
            {
                // 设置 null 时，完全清除持有者，避免陈旧对象
                _tenantContextCurrent.Value = default!;
            }
            else
            {
                // 非空时，创建新持有者替换旧值
                _tenantContextCurrent.Value = new TenantContextHolder { Context = value };
            }
        }
    }


    private sealed class TenantContextHolder
    {
        public ITenantContext? Context;
    }
}
