using Hrevolve.Domain.Common;

namespace Hrevolve.Domain.Employees;

/// <summary>
/// 职位历史 - SCD Type 2 实现
/// 支持员工任意历史时间点状态查询
/// </summary>
public class JobHistory : AuditableEntity
{
    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = null!;
    
    /// <summary>
    /// 生效开始日期
    /// </summary>
    public DateOnly EffectiveStartDate { get; private set; }
    
    /// <summary>
    /// 生效结束日期 - 默认9999-12-31表示当前记录
    /// </summary>
    public DateOnly EffectiveEndDate { get; private set; } = new(9999, 12, 31);
    
    public Guid PositionId { get; private set; }
    public Guid DepartmentId { get; private set; }
    
    /// <summary>
    /// 基本薪资（加密存储）
    /// </summary>
    public decimal BaseSalary { get; private set; }
    
    /// <summary>
    /// 职级
    /// </summary>
    public string? Grade { get; private set; }
    
    /// <summary>
    /// 变更类型
    /// </summary>
    public JobChangeType ChangeType { get; private set; }
    
    /// <summary>
    /// 变更原因
    /// </summary>
    public string? ChangeReason { get; private set; }
    
    /// <summary>
    /// 冲正状态 - 用于历史数据修正
    /// </summary>
    public CorrectionStatus? CorrectionStatus { get; private set; }
    
    /// <summary>
    /// 关联的冲正记录ID
    /// </summary>
    public Guid? CorrectionRefId { get; private set; }
    
    private JobHistory() { }
    
    public static JobHistory Create(
        Guid tenantId,
        Guid employeeId,
        Guid positionId,
        Guid departmentId,
        decimal baseSalary,
        DateOnly effectiveStartDate,
        JobChangeType changeType,
        string? changeReason = null)
    {
        return new JobHistory
        {
            TenantId = tenantId,
            EmployeeId = employeeId,
            PositionId = positionId,
            DepartmentId = departmentId,
            BaseSalary = baseSalary,
            EffectiveStartDate = effectiveStartDate,
            ChangeType = changeType,
            ChangeReason = changeReason
        };
    }
    
    /// <summary>
    /// 关闭当前记录（设置结束日期）
    /// </summary>
    public void Close(DateOnly endDate)
    {
        EffectiveEndDate = endDate;
    }
    
    /// <summary>
    /// 标记为作废（冲正）
    /// </summary>
    public void Void()
    {
        CorrectionStatus = Employees.CorrectionStatus.Voided;
    }
    
    /// <summary>
    /// 判断指定日期是否在有效期内
    /// </summary>
    public bool IsEffectiveOn(DateOnly date)
    {
        return date >= EffectiveStartDate 
               && date <= EffectiveEndDate 
               && CorrectionStatus != Employees.CorrectionStatus.Voided;
    }
}

public enum JobChangeType
{
    NewHire,      // 入职
    Promotion,    // 晋升
    Demotion,     // 降级
    Transfer,     // 调动
    SalaryChange, // 调薪
    Correction    // 数据修正
}

public enum CorrectionStatus
{
    Voided,       // 已作废
    Corrected     // 修正后的记录
}
