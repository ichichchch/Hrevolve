using Hrevolve.Domain.Common;

namespace Hrevolve.Domain.Payroll;

/// <summary>
/// 薪资记录
/// </summary>
public class PayrollRecord : AuditableEntity
{
    public Guid EmployeeId { get; private set; }
    public Guid PayrollPeriodId { get; private set; }
    public PayrollPeriod PayrollPeriod { get; private set; } = null!;
    
    /// <summary>
    /// 基本工资
    /// </summary>
    public decimal BaseSalary { get; private set; }
    
    /// <summary>
    /// 应发工资
    /// </summary>
    public decimal GrossSalary { get; private set; }
    
    /// <summary>
    /// 扣除总额
    /// </summary>
    public decimal TotalDeductions { get; private set; }
    
    /// <summary>
    /// 实发工资
    /// </summary>
    public decimal NetSalary { get; private set; }
    
    /// <summary>
    /// 个人所得税
    /// </summary>
    public decimal IncomeTax { get; private set; }
    
    /// <summary>
    /// 社保个人部分
    /// </summary>
    public decimal SocialInsuranceEmployee { get; private set; }
    
    /// <summary>
    /// 社保公司部分
    /// </summary>
    public decimal SocialInsuranceEmployer { get; private set; }
    
    /// <summary>
    /// 公积金个人部分
    /// </summary>
    public decimal HousingFundEmployee { get; private set; }
    
    /// <summary>
    /// 公积金公司部分
    /// </summary>
    public decimal HousingFundEmployer { get; private set; }
    
    public PayrollRecordStatus Status { get; private set; }
    
    /// <summary>
    /// 计算时间
    /// </summary>
    public DateTime? CalculatedAt { get; private set; }
    
    /// <summary>
    /// 计算日志（JSON）
    /// </summary>
    public string? CalculationLog { get; private set; }
    
    /// <summary>
    /// 数据快照（JSON）- 用于审计
    /// </summary>
    public string? DataSnapshot { get; private set; }
    
    /// <summary>
    /// 薪资明细
    /// </summary>
    private readonly List<PayrollDetail> _details = [];
    public IReadOnlyCollection<PayrollDetail> Details => _details.AsReadOnly();
    
    private PayrollRecord() { }
    
    public static PayrollRecord Create(
        Guid tenantId,
        Guid employeeId,
        Guid payrollPeriodId,
        decimal baseSalary)
    {
        return new PayrollRecord
        {
            TenantId = tenantId,
            EmployeeId = employeeId,
            PayrollPeriodId = payrollPeriodId,
            BaseSalary = baseSalary,
            Status = PayrollRecordStatus.Draft
        };
    }
    
    public void AddDetail(Guid payrollItemId, string itemName, PayrollItemType itemType, decimal amount)
    {
        _details.Add(new PayrollDetail(Id, payrollItemId, itemName, itemType, amount));
    }
    
    public void Calculate()
    {
        GrossSalary = _details.Where(d => d.ItemType == PayrollItemType.Earning).Sum(d => d.Amount);
        TotalDeductions = _details.Where(d => d.ItemType == PayrollItemType.Deduction).Sum(d => d.Amount);
        IncomeTax = _details.Where(d => d.ItemType == PayrollItemType.Tax).Sum(d => d.Amount);
        NetSalary = GrossSalary - TotalDeductions - IncomeTax;
        CalculatedAt = DateTime.UtcNow;
        Status = PayrollRecordStatus.Calculated;
    }
    
    public void Approve()
    {
        Status = PayrollRecordStatus.Approved;
    }
    
    public void MarkAsPaid()
    {
        Status = PayrollRecordStatus.Paid;
    }
}

public enum PayrollRecordStatus
{
    Draft,        // 草稿
    Calculated,   // 已计算
    Approved,     // 已审批
    Paid          // 已发放
}

/// <summary>
/// 薪资明细
/// </summary>
public class PayrollDetail
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid PayrollRecordId { get; private set; }
    public Guid PayrollItemId { get; private set; }
    public string ItemName { get; private set; } = null!;
    public PayrollItemType ItemType { get; private set; }
    public decimal Amount { get; private set; }
    
    /// <summary>
    /// 计算说明
    /// </summary>
    public string? Remarks { get; private set; }
    
    private PayrollDetail() { }
    
    public PayrollDetail(Guid payrollRecordId, Guid payrollItemId, string itemName, PayrollItemType itemType, decimal amount)
    {
        PayrollRecordId = payrollRecordId;
        PayrollItemId = payrollItemId;
        ItemName = itemName;
        ItemType = itemType;
        Amount = amount;
    }
}
