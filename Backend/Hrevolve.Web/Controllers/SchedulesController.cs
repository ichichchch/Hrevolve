namespace Hrevolve.Web.Controllers;

/// <summary>
/// 排班管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SchedulesController : ControllerBase
{
    /// <summary>
    /// 获取排班统计
    /// </summary>
    [HttpGet("stats")]
    public IActionResult GetScheduleStats()
    {
        return Ok(new { totalEmployees = 128, scheduledToday = 98, onDuty = 85, pendingApprovals = 5 });
    }

    /// <summary>
    /// 获取排班列表
    /// </summary>
    [HttpGet]
    public IActionResult GetSchedules([FromQuery] string? startDate, [FromQuery] string? endDate)
    {
        return Ok(new[]
        {
            new { id = Guid.NewGuid(), employeeId = Guid.NewGuid(), employeeName = "张三", departmentName = "研发部", shiftName = "早班", shiftTemplateId = Guid.NewGuid(), date = DateTime.Today.ToString("yyyy-MM-dd"), startTime = "09:00", endTime = "18:00", status = "checked_in" },
            new { id = Guid.NewGuid(), employeeId = Guid.NewGuid(), employeeName = "李四", departmentName = "销售部", shiftName = "早班", shiftTemplateId = Guid.NewGuid(), date = DateTime.Today.ToString("yyyy-MM-dd"), startTime = "09:00", endTime = "18:00", status = "pending" }
        });
    }

    /// <summary>
    /// 获取可排班员工
    /// </summary>
    [HttpGet("schedulable-employees")]
    public IActionResult GetSchedulableEmployees()
    {
        return Ok(new[]
        {
            new { id = Guid.NewGuid(), name = "张三", departmentId = Guid.NewGuid(), departmentName = "研发部" },
            new { id = Guid.NewGuid(), name = "李四", departmentId = Guid.NewGuid(), departmentName = "销售部" },
            new { id = Guid.NewGuid(), name = "王五", departmentId = Guid.NewGuid(), departmentName = "人事部" }
        });
    }

    /// <summary>
    /// 分配排班
    /// </summary>
    [HttpPost("assign")]
    public IActionResult AssignSchedule([FromBody] object data)
    {
        return Ok(new { message = "排班已更新" });
    }

    /// <summary>
    /// 获取班次模板列表
    /// </summary>
    [HttpGet("shift-templates")]
    public IActionResult GetShiftTemplates()
    {
        return Ok(new[]
        {
            new { id = Guid.NewGuid(), code = "MORNING", name = "早班", startTime = "09:00", endTime = "18:00", workHours = 8, breakMinutes = 60, isActive = true },
            new { id = Guid.NewGuid(), code = "AFTERNOON", name = "中班", startTime = "14:00", endTime = "23:00", workHours = 8, breakMinutes = 60, isActive = true },
            new { id = Guid.NewGuid(), code = "NIGHT", name = "晚班", startTime = "22:00", endTime = "07:00", workHours = 8, breakMinutes = 60, isActive = true }
        });
    }

    /// <summary>
    /// 创建班次模板
    /// </summary>
    [HttpPost("shift-templates")]
    public IActionResult CreateShiftTemplate([FromBody] object data)
    {
        return Ok(new { id = Guid.NewGuid(), message = "创建成功" });
    }

    /// <summary>
    /// 更新班次模板
    /// </summary>
    [HttpPut("shift-templates/{id:guid}")]
    public IActionResult UpdateShiftTemplate(Guid id, [FromBody] object data)
    {
        return Ok(new { message = "更新成功" });
    }

    /// <summary>
    /// 删除班次模板
    /// </summary>
    [HttpDelete("shift-templates/{id:guid}")]
    public IActionResult DeleteShiftTemplate(Guid id)
    {
        return Ok(new { message = "删除成功" });
    }
}
