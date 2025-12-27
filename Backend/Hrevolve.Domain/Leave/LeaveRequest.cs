using Hrevolve.Domain.Common;

namespace Hrevolve.Domain.Leave;

/// <summary>
/// 请假申请
/// </summary>
public class LeaveRequest : AuditableEntity
{
    public Guid EmployeeId { get; private set; }
    public Guid LeaveTypeId { get; private set; }
    public LeaveType LeaveType { get; private set; } = null!;
    
    /// <summary>
    /// 开始日期
    /// </summary>
    public DateOnly StartDate { get; private set; }
    
    /// <summary>
    /// 结束日期
    /// </summary>
    public DateOnly EndDate { get; private set; }
    
    /// <summary>
    /// 开始时段（上午/下午/全天）
    /// </summary>
    public DayPart StartDayPart { get; private set; }
    
    /// <summary>
    /// 结束时段
    /// </summary>
    public DayPart EndDayPart { get; private set; }
    
    /// <summary>
    /// 请假天数
    /// </summary>
    public decimal TotalDays { get; private set; }
    
    /// <summary>
    /// 请假原因
    /// </summary>
    public string Reason { get; private set; } = null!;
    
    /// <summary>
    /// 附件URL列表（JSON）
    /// </summary>
    public string? Attachments { get; private set; }
    
    public LeaveRequestStatus Status { get; private set; }
    
    /// <summary>
    /// 审批记录
    /// </summary>
    private readonly List<LeaveApproval> _approvals = [];
    public IReadOnlyCollection<LeaveApproval> Approvals => _approvals.AsReadOnly();
    
    /// <summary>
    /// 取消原因
    /// </summary>
    public string? CancelReason { get; private set; }
    
    private LeaveRequest() { }
    
    public static LeaveRequest Create(
        Guid tenantId,
        Guid employeeId,
        Guid leaveTypeId,
        DateOnly startDate,
        DateOnly endDate,
        DayPart startDayPart,
        DayPart endDayPart,
        string reason)
    {
        var request = new LeaveRequest
        {
            TenantId = tenantId,
            EmployeeId = employeeId,
            LeaveTypeId = leaveTypeId,
            StartDate = startDate,
            EndDate = endDate,
            StartDayPart = startDayPart,
            EndDayPart = endDayPart,
            Reason = reason,
            Status = LeaveRequestStatus.Pending
        };
        
        request.CalculateTotalDays();
        request.AddDomainEvent(new LeaveRequestCreatedEvent(request.Id, tenantId, employeeId));
        return request;
    }
    
    private void CalculateTotalDays()
    {
        var days = (decimal)(EndDate.DayNumber - StartDate.DayNumber) + 1m;
        
        // 处理半天情况
        if (StartDayPart == DayPart.Afternoon) days -= 0.5m;
        if (EndDayPart == DayPart.Morning) days -= 0.5m;
        
        TotalDays = days;
    }
    
    public void Approve(Guid approverId, string? comments = null)
    {
        _approvals.Add(new LeaveApproval(Id, approverId, ApprovalAction.Approve, comments));
        Status = LeaveRequestStatus.Approved;
        AddDomainEvent(new LeaveRequestApprovedEvent(Id, TenantId, EmployeeId));
    }
    
    public void Reject(Guid approverId, string reason)
    {
        _approvals.Add(new LeaveApproval(Id, approverId, ApprovalAction.Reject, reason));
        Status = LeaveRequestStatus.Rejected;
    }
    
    public void Cancel(string reason)
    {
        CancelReason = reason;
        Status = LeaveRequestStatus.Cancelled;
    }
}

public enum DayPart
{
    FullDay,      // 全天
    Morning,      // 上午
    Afternoon     // 下午
}

public enum LeaveRequestStatus
{
    Pending,      // 待审批
    Approved,     // 已批准
    Rejected,     // 已拒绝
    Cancelled     // 已取消
}

/// <summary>
/// 请假审批记录
/// </summary>
public class LeaveApproval
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid LeaveRequestId { get; private set; }
    public Guid ApproverId { get; private set; }
    public ApprovalAction Action { get; private set; }
    public string? Comments { get; private set; }
    public DateTime ApprovedAt { get; private set; } = DateTime.UtcNow;
    
    private LeaveApproval() { }
    
    public LeaveApproval(Guid leaveRequestId, Guid approverId, ApprovalAction action, string? comments)
    {
        LeaveRequestId = leaveRequestId;
        ApproverId = approverId;
        Action = action;
        Comments = comments;
    }
}

public enum ApprovalAction
{
    Approve,
    Reject
}

/// <summary>
/// 假期余额
/// </summary>
public class LeaveBalance : AuditableEntity
{
    public Guid EmployeeId { get; private set; }
    public Guid LeaveTypeId { get; private set; }
    public int Year { get; private set; }
    
    /// <summary>
    /// 年度额度
    /// </summary>
    public decimal Entitlement { get; private set; }
    
    /// <summary>
    /// 结转额度
    /// </summary>
    public decimal CarriedOver { get; private set; }
    
    /// <summary>
    /// 已使用
    /// </summary>
    public decimal Used { get; private set; }
    
    /// <summary>
    /// 待审批
    /// </summary>
    public decimal Pending { get; private set; }
    
    /// <summary>
    /// 可用余额
    /// </summary>
    public decimal Available => Entitlement + CarriedOver - Used - Pending;
    
    private LeaveBalance() { }
    
    public static LeaveBalance Create(
        Guid tenantId,
        Guid employeeId,
        Guid leaveTypeId,
        int year,
        decimal entitlement)
    {
        return new LeaveBalance
        {
            TenantId = tenantId,
            EmployeeId = employeeId,
            LeaveTypeId = leaveTypeId,
            Year = year,
            Entitlement = entitlement
        };
    }
    
    public void AddPending(decimal days) => Pending += days;
    public void RemovePending(decimal days) => Pending -= days;
    public void Use(decimal days)
    {
        Pending -= days;
        Used += days;
    }
    public void Restore(decimal days) => Used -= days;
    public void SetCarryOver(decimal days) => CarriedOver = days;
}

// 领域事件
public record LeaveRequestCreatedEvent(Guid RequestId, Guid TenantId, Guid EmployeeId) : DomainEvent;
public record LeaveRequestApprovedEvent(Guid RequestId, Guid TenantId, Guid EmployeeId) : DomainEvent;
