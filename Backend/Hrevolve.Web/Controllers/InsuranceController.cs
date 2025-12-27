using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrevolve.Web.Controllers;

/// <summary>
/// 保险福利控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InsuranceController : ControllerBase
{
    /// <summary>
    /// 获取保险统计
    /// </summary>
    [HttpGet("stats")]
    public IActionResult GetInsuranceStats()
    {
        return Ok(new { totalPlans = 5, enrolledEmployees = 120, monthlyPremium = 156000m, pendingClaims = 3 });
    }

    /// <summary>
    /// 获取保险方案列表
    /// </summary>
    [HttpGet("plans")]
    public IActionResult GetInsurancePlans()
    {
        return Ok(new[]
        {
            new { id = Guid.NewGuid(), name = "基本医疗保险", type = "health", premium = 500m, coverage = 500000m, isActive = true, description = "基本医疗保障" },
            new { id = Guid.NewGuid(), name = "补充医疗保险", type = "health", premium = 200m, coverage = 200000m, isActive = true, description = "补充医疗保障" },
            new { id = Guid.NewGuid(), name = "意外伤害保险", type = "accident", premium = 100m, coverage = 1000000m, isActive = true, description = "意外伤害保障" }
        });
    }

    /// <summary>
    /// 创建保险方案
    /// </summary>
    [HttpPost("plans")]
    public IActionResult CreateInsurancePlan([FromBody] object data)
    {
        return Ok(new { id = Guid.NewGuid(), message = "创建成功" });
    }

    /// <summary>
    /// 更新保险方案
    /// </summary>
    [HttpPut("plans/{id:guid}")]
    public IActionResult UpdateInsurancePlan(Guid id, [FromBody] object data)
    {
        return Ok(new { message = "更新成功" });
    }

    /// <summary>
    /// 删除保险方案
    /// </summary>
    [HttpDelete("plans/{id:guid}")]
    public IActionResult DeleteInsurancePlan(Guid id)
    {
        return Ok(new { message = "删除成功" });
    }

    /// <summary>
    /// 获取员工参保列表
    /// </summary>
    [HttpGet("employees")]
    public IActionResult GetEmployeeInsurances([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        return Ok(new
        {
            items = new[]
            {
                new { id = Guid.NewGuid(), employeeId = Guid.NewGuid(), employeeName = "张三", planId = Guid.NewGuid(), planName = "基本医疗保险", startDate = "2024-01-01", premium = 500m, status = "active" },
                new { id = Guid.NewGuid(), employeeId = Guid.NewGuid(), employeeName = "李四", planId = Guid.NewGuid(), planName = "基本医疗保险", startDate = "2024-01-01", premium = 500m, status = "active" }
            },
            total = 2,
            page,
            pageSize
        });
    }

    /// <summary>
    /// 员工参保
    /// </summary>
    [HttpPost("enroll")]
    public IActionResult EnrollEmployeeInsurance([FromBody] object data)
    {
        return Ok(new { id = Guid.NewGuid(), message = "参保成功" });
    }

    /// <summary>
    /// 终止员工保险
    /// </summary>
    [HttpPost("employees/{id:guid}/terminate")]
    public IActionResult TerminateEmployeeInsurance(Guid id)
    {
        return Ok(new { message = "已终止" });
    }

    /// <summary>
    /// 获取福利项目列表
    /// </summary>
    [HttpGet("benefits-simple")]
    public IActionResult GetBenefits()
    {
        return Ok(new[]
        {
            new { id = Guid.NewGuid(), name = "餐饮补贴", type = "meal", amount = 500m, isActive = true, description = "每月餐饮补贴" },
            new { id = Guid.NewGuid(), name = "交通补贴", type = "transport", amount = 300m, isActive = true, description = "每月交通补贴" },
            new { id = Guid.NewGuid(), name = "通讯补贴", type = "communication", amount = 200m, isActive = true, description = "每月通讯补贴" }
        });
    }

    /// <summary>
    /// 创建福利项目
    /// </summary>
    [HttpPost("benefits-simple")]
    public IActionResult CreateBenefit([FromBody] object data)
    {
        return Ok(new { id = Guid.NewGuid(), message = "创建成功" });
    }

    /// <summary>
    /// 更新福利项目
    /// </summary>
    [HttpPut("benefits-simple/{id:guid}")]
    public IActionResult UpdateBenefit(Guid id, [FromBody] object data)
    {
        return Ok(new { message = "更新成功" });
    }

    /// <summary>
    /// 删除福利项目
    /// </summary>
    [HttpDelete("benefits-simple/{id:guid}")]
    public IActionResult DeleteBenefit(Guid id)
    {
        return Ok(new { message = "删除成功" });
    }
}
