using Hrevolve.Shared.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Hrevolve.Web.Filters;

/// <summary>
/// 权限验证特性 - 检查用户是否具有指定权限
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequirePermissionAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _permissions;
    private readonly bool _requireAll;
    
    /// <summary>
    /// 创建权限验证特性
    /// </summary>
    /// <param name="permissions">所需权限列表</param>
    public RequirePermissionAttribute(params string[] permissions)
    {
        _permissions = permissions;
        _requireAll = false;
    }
    
    /// <summary>
    /// 创建权限验证特性
    /// </summary>
    /// <param name="requireAll">是否需要全部权限</param>
    /// <param name="permissions">所需权限列表</param>
    public RequirePermissionAttribute(bool requireAll, params string[] permissions)
    {
        _permissions = permissions;
        _requireAll = requireAll;
    }
    
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var currentUserAccessor = context.HttpContext.RequestServices.GetService<ICurrentUserAccessor>();
        var currentUser = currentUserAccessor?.CurrentUser;
        
        if (currentUser == null || !currentUser.IsAuthenticated)
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                code = "UNAUTHORIZED",
                message = "未授权访问"
            });
            return;
        }
        
        // 系统管理员拥有所有权限
        if (currentUser.HasPermission("system:admin"))
        {
            return;
        }
        
        bool hasPermission;
        
        if (_requireAll)
        {
            // 需要全部权限
            hasPermission = _permissions.All(p => currentUser.HasPermission(p));
        }
        else
        {
            // 只需要任一权限
            hasPermission = _permissions.Any(p => currentUser.HasPermission(p));
        }
        
        if (!hasPermission)
        {
            context.Result = new ObjectResult(new
            {
                code = "FORBIDDEN",
                message = "权限不足"
            })
            {
                StatusCode = 403
            };
        }
    }
}
