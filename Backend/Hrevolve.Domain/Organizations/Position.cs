using Hrevolve.Domain.Common;

namespace Hrevolve.Domain.Organizations;

/// <summary>
/// 职位
/// </summary>
public class Position : AuditableEntity
{
    public string Name { get; private set; } = null!;
    public string Code { get; private set; } = null!;
    public string? Description { get; private set; }
    public Guid OrganizationUnitId { get; private set; }
    public OrganizationUnit OrganizationUnit { get; private set; } = null!;
    public PositionLevel Level { get; private set; }
    public bool IsActive { get; private set; } = true;
    
    /// <summary>
    /// 职位序列（如：技术序列、管理序列）
    /// </summary>
    public string? Sequence { get; private set; }
    
    /// <summary>
    /// 薪资范围下限
    /// </summary>
    public decimal? SalaryRangeMin { get; private set; }
    
    /// <summary>
    /// 薪资范围上限
    /// </summary>
    public decimal? SalaryRangeMax { get; private set; }
    
    private Position() { }
    
    public static Position Create(
        Guid tenantId,
        string name,
        string code,
        Guid organizationUnitId,
        PositionLevel level)
    {
        return new Position
        {
            TenantId = tenantId,
            Name = name,
            Code = code,
            OrganizationUnitId = organizationUnitId,
            Level = level
        };
    }
    
    public void SetSalaryRange(decimal min, decimal max)
    {
        SalaryRangeMin = min;
        SalaryRangeMax = max;
    }
    
    public void Deactivate() => IsActive = false;
}

public enum PositionLevel
{
    Entry,        // 初级
    Junior,       // 中级
    Senior,       // 高级
    Lead,         // 主管
    Manager,      // 经理
    Director,     // 总监
    VP,           // 副总裁
    CLevel        // C级高管
}
