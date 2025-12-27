using Hrevolve.Domain.Common;

namespace Hrevolve.Domain.Leave;

/// <summary>
/// 假期类型
/// </summary>
public class LeaveType : AuditableEntity
{
    public string Name { get; private set; } = null!;
    public string Code { get; private set; } = null!;
    public string? Description { get; private set; }
    
    /// <summary>
    /// 是否带薪
    /// </summary>
    public bool IsPaid { get; private set; }
    
    /// <summary>
    /// 是否需要审批
    /// </summary>
    public bool RequiresApproval { get; private set; } = true;
    
    /// <summary>
    /// 是否允许半天
    /// </summary>
    public bool AllowHalfDay { get; private set; }
    
    /// <summary>
    /// 最小请假单位（小时）
    /// </summary>
    public decimal MinUnit { get; private set; } = 4;
    
    /// <summary>
    /// 单次最大天数
    /// </summary>
    public int? MaxDaysPerRequest { get; private set; }
    
    /// <summary>
    /// 是否需要附件
    /// </summary>
    public bool RequiresAttachment { get; private set; }
    
    /// <summary>
    /// 颜色标识
    /// </summary>
    public string? Color { get; private set; }
    
    public bool IsActive { get; private set; } = true;
    
    private LeaveType() { }
    
    public static LeaveType Create(
        Guid tenantId,
        string name,
        string code,
        bool isPaid)
    {
        return new LeaveType
        {
            TenantId = tenantId,
            Name = name,
            Code = code,
            IsPaid = isPaid
        };
    }
}

/// <summary>
/// 假期策略 - 配置假期额度规则
/// </summary>
public class LeavePolicy : AuditableEntity
{
    public Guid LeaveTypeId { get; private set; }
    public LeaveType LeaveType { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    
    /// <summary>
    /// 入职后多少天生效
    /// </summary>
    public int EffectiveAfterDays { get; private set; }
    
    /// <summary>
    /// 年度基础额度（天）
    /// </summary>
    public decimal BaseQuota { get; private set; }
    
    /// <summary>
    /// 按司龄增加额度规则（JSON）
    /// 如: [{"minYears":1,"maxYears":5,"extraDays":1},{"minYears":5,"maxYears":10,"extraDays":2}]
    /// </summary>
    public string? SeniorityRules { get; private set; }
    
    /// <summary>
    /// 按职级增加额度规则（JSON）
    /// </summary>
    public string? GradeRules { get; private set; }
    
    /// <summary>
    /// 最大结转天数
    /// </summary>
    public decimal MaxCarryOverDays { get; private set; }
    
    /// <summary>
    /// 结转有效期（月）
    /// </summary>
    public int CarryOverExpiryMonths { get; private set; }
    
    /// <summary>
    /// 是否按比例发放（入职当年）
    /// </summary>
    public bool ProRataFirstYear { get; private set; } = true;
    
    /// <summary>
    /// 额度发放周期
    /// </summary>
    public AccrualPeriod AccrualPeriod { get; private set; }
    
    public bool IsActive { get; private set; } = true;
    
    private LeavePolicy() { }
    
    public static LeavePolicy Create(
        Guid tenantId,
        Guid leaveTypeId,
        string name,
        decimal baseQuota)
    {
        return new LeavePolicy
        {
            TenantId = tenantId,
            LeaveTypeId = leaveTypeId,
            Name = name,
            BaseQuota = baseQuota,
            AccrualPeriod = AccrualPeriod.Yearly
        };
    }
}

public enum AccrualPeriod
{
    Yearly,       // 年度一次性发放
    Monthly,      // 按月发放
    Quarterly     // 按季度发放
}
