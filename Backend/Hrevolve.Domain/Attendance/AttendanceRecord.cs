using Hrevolve.Domain.Common;

namespace Hrevolve.Domain.Attendance;

/// <summary>
/// 考勤记录
/// </summary>
public class AttendanceRecord : AuditableEntity
{
    public Guid EmployeeId { get; private set; }
    public DateOnly AttendanceDate { get; private set; }
    public Guid? ScheduleId { get; private set; }
    public Schedule? Schedule { get; private set; }
    
    /// <summary>
    /// 签到时间
    /// </summary>
    public DateTime? CheckInTime { get; private set; }
    
    /// <summary>
    /// 签退时间
    /// </summary>
    public DateTime? CheckOutTime { get; private set; }
    
    /// <summary>
    /// 签到方式
    /// </summary>
    public CheckInMethod? CheckInMethod { get; private set; }
    
    /// <summary>
    /// 签退方式
    /// </summary>
    public CheckInMethod? CheckOutMethod { get; private set; }
    
    /// <summary>
    /// 签到位置（GPS坐标）
    /// </summary>
    public string? CheckInLocation { get; private set; }
    
    /// <summary>
    /// 签退位置
    /// </summary>
    public string? CheckOutLocation { get; private set; }
    
    /// <summary>
    /// 考勤状态
    /// </summary>
    public AttendanceStatus Status { get; private set; }
    
    /// <summary>
    /// 迟到分钟数
    /// </summary>
    public int LateMinutes { get; private set; }
    
    /// <summary>
    /// 早退分钟数
    /// </summary>
    public int EarlyLeaveMinutes { get; private set; }
    
    /// <summary>
    /// 实际工时（小时）
    /// </summary>
    public decimal ActualHours { get; private set; }
    
    /// <summary>
    /// 加班时长（小时）
    /// </summary>
    public decimal OvertimeHours { get; private set; }
    
    /// <summary>
    /// 备注
    /// </summary>
    public string? Remarks { get; private set; }
    
    /// <summary>
    /// 是否已审核
    /// </summary>
    public bool IsApproved { get; private set; }
    
    public Guid? ApprovedBy { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    
    private AttendanceRecord() { }
    
    public static AttendanceRecord Create(
        Guid tenantId,
        Guid employeeId,
        DateOnly attendanceDate,
        Guid? scheduleId = null)
    {
        return new AttendanceRecord
        {
            TenantId = tenantId,
            EmployeeId = employeeId,
            AttendanceDate = attendanceDate,
            ScheduleId = scheduleId,
            Status = AttendanceStatus.Pending
        };
    }
    
    public void CheckIn(DateTime time, CheckInMethod method, string? location = null)
    {
        CheckInTime = time;
        CheckInMethod = method;
        CheckInLocation = location;
        UpdateStatus();
    }
    
    public void CheckOut(DateTime time, CheckInMethod method, string? location = null)
    {
        CheckOutTime = time;
        CheckOutMethod = method;
        CheckOutLocation = location;
        CalculateHours();
        UpdateStatus();
    }
    
    /// <summary>
    /// 补卡
    /// </summary>
    public void ManualCheckIn(DateTime time, string remarks)
    {
        CheckInTime = time;
        CheckInMethod = Attendance.CheckInMethod.Manual;
        Remarks = remarks;
        UpdateStatus();
    }
    
    public void ManualCheckOut(DateTime time, string remarks)
    {
        CheckOutTime = time;
        CheckOutMethod = Attendance.CheckInMethod.Manual;
        Remarks = string.IsNullOrEmpty(Remarks) ? remarks : $"{Remarks}; {remarks}";
        CalculateHours();
        UpdateStatus();
    }
    
    private void CalculateHours()
    {
        if (CheckInTime.HasValue && CheckOutTime.HasValue)
        {
            var duration = CheckOutTime.Value - CheckInTime.Value;
            ActualHours = (decimal)duration.TotalHours;
        }
    }
    
    private void UpdateStatus()
    {
        if (!CheckInTime.HasValue && !CheckOutTime.HasValue)
        {
            Status = AttendanceStatus.Absent;
        }
        else if (CheckInTime.HasValue && CheckOutTime.HasValue)
        {
            Status = AttendanceStatus.Normal;
        }
        else
        {
            Status = AttendanceStatus.Incomplete;
        }
    }
    
    public void Approve(Guid approvedBy)
    {
        IsApproved = true;
        ApprovedBy = approvedBy;
        ApprovedAt = DateTime.UtcNow;
    }
}

public enum CheckInMethod
{
    App,          // App打卡（GPS）
    WiFi,         // WiFi打卡
    Device,       // 考勤机
    Manual,       // 手动补卡
    Web           // Web打卡
}

public enum AttendanceStatus
{
    Pending,      // 待处理
    Normal,       // 正常
    Late,         // 迟到
    EarlyLeave,   // 早退
    Absent,       // 缺勤
    Incomplete,   // 打卡不完整
    Leave,        // 请假
    BusinessTrip  // 出差
}
