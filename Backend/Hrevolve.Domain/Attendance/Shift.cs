using Hrevolve.Domain.Common;

namespace Hrevolve.Domain.Attendance;

/// <summary>
/// 班次定义
/// </summary>
public class Shift : AuditableEntity
{
    public string Name { get; private set; } = null!;
    public string Code { get; private set; } = null!;
    public string? Description { get; private set; }
    
    /// <summary>
    /// 上班时间
    /// </summary>
    public TimeOnly StartTime { get; private set; }
    
    /// <summary>
    /// 下班时间
    /// </summary>
    public TimeOnly EndTime { get; private set; }
    
    /// <summary>
    /// 是否跨天
    /// </summary>
    public bool CrossDay { get; private set; }
    
    /// <summary>
    /// 标准工时（小时）
    /// </summary>
    public decimal StandardHours { get; private set; }
    
    /// <summary>
    /// 弹性上班时间（分钟）
    /// </summary>
    public int FlexibleStartMinutes { get; private set; }
    
    /// <summary>
    /// 弹性下班时间（分钟）
    /// </summary>
    public int FlexibleEndMinutes { get; private set; }
    
    /// <summary>
    /// 休息开始时间
    /// </summary>
    public TimeOnly? BreakStartTime { get; private set; }
    
    /// <summary>
    /// 休息结束时间
    /// </summary>
    public TimeOnly? BreakEndTime { get; private set; }
    
    public bool IsActive { get; private set; } = true;
    
    private Shift() { }
    
    public static Shift Create(
        Guid tenantId,
        string name,
        string code,
        TimeOnly startTime,
        TimeOnly endTime,
        decimal standardHours)
    {
        return new Shift
        {
            TenantId = tenantId,
            Name = name,
            Code = code,
            StartTime = startTime,
            EndTime = endTime,
            StandardHours = standardHours,
            CrossDay = endTime < startTime
        };
    }
    
    public void SetFlexibleTime(int startMinutes, int endMinutes)
    {
        FlexibleStartMinutes = startMinutes;
        FlexibleEndMinutes = endMinutes;
    }
    
    public void SetBreakTime(TimeOnly start, TimeOnly end)
    {
        BreakStartTime = start;
        BreakEndTime = end;
    }
}

/// <summary>
/// 排班记录
/// </summary>
public class Schedule : AuditableEntity
{
    public Guid EmployeeId { get; private set; }
    public Guid ShiftId { get; private set; }
    public Shift Shift { get; private set; } = null!;
    public DateOnly ScheduleDate { get; private set; }
    
    /// <summary>
    /// 是否休息日
    /// </summary>
    public bool IsRestDay { get; private set; }
    
    /// <summary>
    /// 备注
    /// </summary>
    public string? Remarks { get; private set; }
    
    private Schedule() { }
    
    public static Schedule Create(
        Guid tenantId,
        Guid employeeId,
        Guid shiftId,
        DateOnly scheduleDate,
        bool isRestDay = false)
    {
        return new Schedule
        {
            TenantId = tenantId,
            EmployeeId = employeeId,
            ShiftId = shiftId,
            ScheduleDate = scheduleDate,
            IsRestDay = isRestDay
        };
    }
}
