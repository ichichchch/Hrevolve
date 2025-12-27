using Hrevolve.Domain.Common;

namespace Hrevolve.Domain.Payroll;

/// <summary>
/// 薪资周期
/// </summary>
public class PayrollPeriod : AuditableEntity
{
    public string Name { get; private set; } = null!;
    public int Year { get; private set; }
    public int Month { get; private set; }
    
    /// <summary>
    /// 周期开始日期
    /// </summary>
    public DateOnly StartDate { get; private set; }
    
    /// <summary>
    /// 周期结束日期
    /// </summary>
    public DateOnly EndDate { get; private set; }
    
    /// <summary>
    /// 发薪日期
    /// </summary>
    public DateOnly PayDate { get; private set; }
    
    public PayrollPeriodStatus Status { get; private set; }
    
    /// <summary>
    /// 锁定时间
    /// </summary>
    public DateTime? LockedAt { get; private set; }
    
    public Guid? LockedBy { get; private set; }
    
    private PayrollPeriod() { }
    
    public static PayrollPeriod Create(
        Guid tenantId,
        int year,
        int month,
        DateOnly startDate,
        DateOnly endDate,
        DateOnly payDate)
    {
        return new PayrollPeriod
        {
            TenantId = tenantId,
            Name = $"{year}年{month}月",
            Year = year,
            Month = month,
            StartDate = startDate,
            EndDate = endDate,
            PayDate = payDate,
            Status = PayrollPeriodStatus.Open
        };
    }
    
    public void Lock(Guid userId)
    {
        Status = PayrollPeriodStatus.Locked;
        LockedAt = DateTime.UtcNow;
        LockedBy = userId;
    }
    
    public void Close()
    {
        Status = PayrollPeriodStatus.Closed;
    }
}

public enum PayrollPeriodStatus
{
    Open,         // 开放（可编辑）
    Processing,   // 计算中
    Locked,       // 已锁定（待发放）
    Closed        // 已关闭
}

/// <summary>
/// 薪资项定义
/// </summary>
public class PayrollItem : AuditableEntity
{
    public string Name { get; private set; } = null!;
    public string Code { get; private set; } = null!;
    public string? Description { get; private set; }
    
    /// <summary>
    /// 薪资项类型
    /// </summary>
    public PayrollItemType Type { get; private set; }
    
    /// <summary>
    /// 计算类型
    /// </summary>
    public CalculationType CalculationType { get; private set; }
    
    /// <summary>
    /// 固定金额（当CalculationType为Fixed时）
    /// </summary>
    public decimal? FixedAmount { get; private set; }
    
    /// <summary>
    /// 计算公式（当CalculationType为Formula时）
    /// </summary>
    public string? Formula { get; private set; }
    
    /// <summary>
    /// 是否计税
    /// </summary>
    public bool IsTaxable { get; private set; }
    
    /// <summary>
    /// 排序顺序
    /// </summary>
    public int SortOrder { get; private set; }
    
    public bool IsActive { get; private set; } = true;
    
    private PayrollItem() { }
    
    public static PayrollItem Create(
        Guid tenantId,
        string name,
        string code,
        PayrollItemType type,
        CalculationType calculationType)
    {
        return new PayrollItem
        {
            TenantId = tenantId,
            Name = name,
            Code = code,
            Type = type,
            CalculationType = calculationType
        };
    }
}

public enum PayrollItemType
{
    Earning,      // 收入项
    Deduction,    // 扣除项
    Benefit,      // 福利项
    Tax           // 税项
}

public enum CalculationType
{
    Fixed,        // 固定金额
    Formula,      // 公式计算
    Manual,       // 手动输入
    External      // 外部数据
}
