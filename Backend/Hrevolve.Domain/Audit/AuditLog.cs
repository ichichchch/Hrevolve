using Hrevolve.Domain.Common;

namespace Hrevolve.Domain.Audit;

/// <summary>
/// 审计日志 - 记录所有操作
/// </summary>
public class AuditLog : Entity
{
    public Guid TenantId { get; private set; }
    public Guid? UserId { get; private set; }
    public string? UserName { get; private set; }
    
    /// <summary>
    /// 操作类型
    /// </summary>
    public string Action { get; private set; } = null!;
    
    /// <summary>
    /// 实体类型
    /// </summary>
    public string EntityType { get; private set; } = null!;
    
    /// <summary>
    /// 实体ID
    /// </summary>
    public string? EntityId { get; private set; }
    
    /// <summary>
    /// 旧值（JSON）
    /// </summary>
    public string? OldValues { get; private set; }
    
    /// <summary>
    /// 新值（JSON）
    /// </summary>
    public string? NewValues { get; private set; }
    
    /// <summary>
    /// 变更字段列表
    /// </summary>
    public string? AffectedColumns { get; private set; }
    
    /// <summary>
    /// 客户端IP
    /// </summary>
    public string? IpAddress { get; private set; }
    
    /// <summary>
    /// 用户代理
    /// </summary>
    public string? UserAgent { get; private set; }
    
    /// <summary>
    /// 请求路径
    /// </summary>
    public string? RequestPath { get; private set; }
    
    /// <summary>
    /// 追踪ID
    /// </summary>
    public string? TraceId { get; private set; }
    
    /// <summary>
    /// 关联ID
    /// </summary>
    public string? CorrelationId { get; private set; }
    
    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;
    
    private AuditLog() { }
    
    public static AuditLog Create(
        Guid tenantId,
        Guid? userId,
        string? userName,
        string action,
        string entityType,
        string? entityId = null)
    {
        return new AuditLog
        {
            TenantId = tenantId,
            UserId = userId,
            UserName = userName,
            Action = action,
            EntityType = entityType,
            EntityId = entityId
        };
    }
    
    public void SetChanges(string? oldValues, string? newValues, string? affectedColumns)
    {
        OldValues = oldValues;
        NewValues = newValues;
        AffectedColumns = affectedColumns;
    }
    
    public void SetRequestInfo(string? ipAddress, string? userAgent, string? requestPath, string? traceId, string? correlationId)
    {
        IpAddress = ipAddress;
        UserAgent = userAgent;
        RequestPath = requestPath;
        TraceId = traceId;
        CorrelationId = correlationId;
    }
}

/// <summary>
/// 审计操作类型常量
/// </summary>
public static class AuditActions
{
    public const string Create = "Create";
    public const string Update = "Update";
    public const string Delete = "Delete";
    public const string Login = "Login";
    public const string Logout = "Logout";
    public const string LoginFailed = "LoginFailed";
    public const string PasswordChange = "PasswordChange";
    public const string MfaEnabled = "MfaEnabled";
    public const string MfaDisabled = "MfaDisabled";
    public const string Export = "Export";
    public const string Import = "Import";
}
