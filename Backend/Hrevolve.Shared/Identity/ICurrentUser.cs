namespace Hrevolve.Shared.Identity;

/// <summary>
/// 当前用户接口 - 提供当前请求的用户信息
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// 用户ID
    /// </summary>
    Guid? Id { get; }
    
    /// <summary>
    /// 用户名
    /// </summary>
    string? UserName { get; }
    
    /// <summary>
    /// 邮箱
    /// </summary>
    string? Email { get; }
    
    /// <summary>
    /// 租户ID
    /// </summary>
    Guid? TenantId { get; }
    
    /// <summary>
    /// 员工ID
    /// </summary>
    Guid? EmployeeId { get; }
    
    /// <summary>
    /// 角色列表
    /// </summary>
    IEnumerable<string> Roles { get; }
    
    /// <summary>
    /// 权限列表
    /// </summary>
    IEnumerable<string> Permissions { get; }
    
    /// <summary>
    /// 是否已认证
    /// </summary>
    bool IsAuthenticated { get; }
    
    /// <summary>
    /// 检查是否有指定权限
    /// </summary>
    bool HasPermission(string permission);
    
    /// <summary>
    /// 检查是否有指定角色
    /// </summary>
    bool IsInRole(string role);
}

/// <summary>
/// 当前用户访问器
/// </summary>
public interface ICurrentUserAccessor
{
    ICurrentUser? CurrentUser { get; set; }
}

/// <summary>
/// 当前用户实现
/// </summary>
public class CurrentUser : ICurrentUser
{
    public Guid? Id { get; init; }
    public string? UserName { get; init; }
    public string? Email { get; init; }
    public Guid? TenantId { get; init; }
    public Guid? EmployeeId { get; init; }
    public IEnumerable<string> Roles { get; init; } = [];
    public IEnumerable<string> Permissions { get; init; } = [];
    public bool IsAuthenticated => Id.HasValue;
    
    public bool HasPermission(string permission) => Permissions.Contains(permission);
    public bool IsInRole(string role) => Roles.Contains(role);
}

/// <summary>
/// 当前用户访问器实现
/// </summary>
public class CurrentUserAccessor : ICurrentUserAccessor
{
    private static readonly AsyncLocal<CurrentUserHolder> _currentUserHolder = new();
    
    public ICurrentUser? CurrentUser
    {
        get => _currentUserHolder.Value?.User;
        set
        {
            var holder = _currentUserHolder.Value;
            if (holder != null)
            {
                holder.User = null;
            }
            
            if (value != null)
            {
                _currentUserHolder.Value = new CurrentUserHolder { User = value };
            }
        }
    }
    
    private class CurrentUserHolder
    {
        public ICurrentUser? User;
    }
}
