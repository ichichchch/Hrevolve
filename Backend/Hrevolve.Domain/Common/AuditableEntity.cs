namespace Hrevolve.Domain.Common;

/// <summary>
/// 可审计实体基类 - 包含租户隔离和审计字段
/// </summary>
public abstract class AuditableEntity : Entity
{
    /// <summary>
    /// 租户ID - 多租户隔离
    /// </summary>
    public Guid TenantId { get; set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// 创建人ID
    /// </summary>
    public Guid? CreatedBy { get; set; }
    
    /// <summary>
    /// 最后修改时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// 最后修改人ID
    /// </summary>
    public Guid? UpdatedBy { get; set; }
    
    /// <summary>
    /// 软删除标记
    /// </summary>
    public bool IsDeleted { get; set; }
    
    /// <summary>
    /// 删除时间
    /// </summary>
    public DateTime? DeletedAt { get; set; }
    
    /// <summary>
    /// 删除人ID
    /// </summary>
    public Guid? DeletedBy { get; set; }
}
