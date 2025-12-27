using Hrevolve.Domain.Common;

namespace Hrevolve.Domain.Expense;

/// <summary>
/// 报销申请
/// </summary>
public class ExpenseRequest : AuditableEntity
{
    public Guid EmployeeId { get; private set; }
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    
    /// <summary>
    /// 报销总金额
    /// </summary>
    public decimal TotalAmount { get; private set; }
    
    /// <summary>
    /// 币种
    /// </summary>
    public string Currency { get; private set; } = "CNY";
    
    public ExpenseRequestStatus Status { get; private set; }
    
    /// <summary>
    /// 是否计入工资
    /// </summary>
    public bool IncludeInPayroll { get; private set; }
    
    /// <summary>
    /// 关联的薪资周期ID
    /// </summary>
    public Guid? PayrollPeriodId { get; private set; }
    
    /// <summary>
    /// 报销明细
    /// </summary>
    private readonly List<ExpenseItem> _items = [];
    public IReadOnlyCollection<ExpenseItem> Items => _items.AsReadOnly();
    
    /// <summary>
    /// 审批记录
    /// </summary>
    private readonly List<ExpenseApproval> _approvals = [];
    public IReadOnlyCollection<ExpenseApproval> Approvals => _approvals.AsReadOnly();
    
    private ExpenseRequest() { }
    
    public static ExpenseRequest Create(
        Guid tenantId,
        Guid employeeId,
        string title)
    {
        return new ExpenseRequest
        {
            TenantId = tenantId,
            EmployeeId = employeeId,
            Title = title,
            Status = ExpenseRequestStatus.Draft
        };
    }
    
    public void AddItem(ExpenseCategory category, decimal amount, DateOnly expenseDate, string? description, string? receiptUrl)
    {
        var item = new ExpenseItem(Id, category, amount, expenseDate, description, receiptUrl);
        _items.Add(item);
        RecalculateTotal();
    }
    
    private void RecalculateTotal()
    {
        TotalAmount = _items.Sum(i => i.Amount);
    }
    
    public void Submit()
    {
        if (_items.Count == 0)
            throw new InvalidOperationException("报销申请必须包含至少一项费用");
        
        Status = ExpenseRequestStatus.Pending;
    }
    
    public void Approve(Guid approverId, string? comments = null)
    {
        _approvals.Add(new ExpenseApproval(Id, approverId, ExpenseApprovalAction.Approve, comments));
        Status = ExpenseRequestStatus.Approved;
    }
    
    public void Reject(Guid approverId, string reason)
    {
        _approvals.Add(new ExpenseApproval(Id, approverId, ExpenseApprovalAction.Reject, reason));
        Status = ExpenseRequestStatus.Rejected;
    }
    
    public void MarkAsPaid(Guid? payrollPeriodId = null)
    {
        Status = ExpenseRequestStatus.Paid;
        if (payrollPeriodId.HasValue)
        {
            IncludeInPayroll = true;
            PayrollPeriodId = payrollPeriodId;
        }
    }
}

public enum ExpenseRequestStatus
{
    Draft,        // 草稿
    Pending,      // 待审批
    Approved,     // 已批准
    Rejected,     // 已拒绝
    Paid          // 已打款
}

/// <summary>
/// 报销明细
/// </summary>
public class ExpenseItem
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ExpenseRequestId { get; private set; }
    public ExpenseCategory Category { get; private set; }
    public decimal Amount { get; private set; }
    public DateOnly ExpenseDate { get; private set; }
    public string? Description { get; private set; }
    
    /// <summary>
    /// 发票/收据URL
    /// </summary>
    public string? ReceiptUrl { get; private set; }
    
    private ExpenseItem() { }
    
    public ExpenseItem(Guid expenseRequestId, ExpenseCategory category, decimal amount, DateOnly expenseDate, string? description, string? receiptUrl)
    {
        ExpenseRequestId = expenseRequestId;
        Category = category;
        Amount = amount;
        ExpenseDate = expenseDate;
        Description = description;
        ReceiptUrl = receiptUrl;
    }
}

public enum ExpenseCategory
{
    Travel,           // 差旅
    Meals,            // 餐饮
    Transportation,   // 交通
    Accommodation,    // 住宿
    Communication,    // 通讯
    Office,           // 办公用品
    Training,         // 培训
    Entertainment,    // 招待
    Other             // 其他
}

/// <summary>
/// 报销审批记录
/// </summary>
public class ExpenseApproval
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ExpenseRequestId { get; private set; }
    public Guid ApproverId { get; private set; }
    public ExpenseApprovalAction Action { get; private set; }
    public string? Comments { get; private set; }
    public DateTime ApprovedAt { get; private set; } = DateTime.UtcNow;
    
    private ExpenseApproval() { }
    
    public ExpenseApproval(Guid expenseRequestId, Guid approverId, ExpenseApprovalAction action, string? comments)
    {
        ExpenseRequestId = expenseRequestId;
        ApproverId = approverId;
        Action = action;
        Comments = comments;
    }
}

public enum ExpenseApprovalAction
{
    Approve,
    Reject
}
