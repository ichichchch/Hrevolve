namespace Hrevolve.Shared.Exceptions;

/// <summary>
/// 业务异常基类
/// </summary>
public class HrevolveException : Exception
{
    public string ErrorCode { get; }
    
    public HrevolveException(string message, string errorCode = "BUSINESS_ERROR")
        : base(message)
    {
        ErrorCode = errorCode;
    }
    
    public HrevolveException(string message, string errorCode, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}

/// <summary>
/// 实体未找到异常
/// </summary>
public class EntityNotFoundException : HrevolveException
{
    public string EntityType { get; }
    public object? EntityId { get; }
    
    public EntityNotFoundException(string entityType, object? entityId = null)
        : base($"实体 '{entityType}' 未找到" + (entityId != null ? $"，ID: {entityId}" : ""), "ENTITY_NOT_FOUND")
    {
        EntityType = entityType;
        EntityId = entityId;
    }
}

/// <summary>
/// 验证异常
/// </summary>
public class ValidationException : HrevolveException
{
    public IDictionary<string, string[]> Errors { get; }
    
    public ValidationException(IDictionary<string, string[]> errors)
        : base("验证失败", "VALIDATION_ERROR")
    {
        Errors = errors;
    }
    
    public ValidationException(string field, string message)
        : base(message, "VALIDATION_ERROR")
    {
        Errors = new Dictionary<string, string[]> { { field, [message] } };
    }
}

/// <summary>
/// 权限不足异常
/// </summary>
public class ForbiddenException : HrevolveException
{
    public ForbiddenException(string message = "权限不足")
        : base(message, "FORBIDDEN")
    {
    }
}

/// <summary>
/// 未授权异常
/// </summary>
public class UnauthorizedException : HrevolveException
{
    public UnauthorizedException(string message = "未授权访问")
        : base(message, "UNAUTHORIZED")
    {
    }
}

/// <summary>
/// 并发冲突异常
/// </summary>
public class ConcurrencyException : HrevolveException
{
    public ConcurrencyException(string message = "数据已被其他用户修改，请刷新后重试")
        : base(message, "CONCURRENCY_CONFLICT")
    {
    }
}

/// <summary>
/// 租户异常
/// </summary>
public class TenantException : HrevolveException
{
    public TenantException(string message)
        : base(message, "TENANT_ERROR")
    {
    }
}
