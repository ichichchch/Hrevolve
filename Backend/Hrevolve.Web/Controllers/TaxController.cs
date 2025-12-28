namespace Hrevolve.Web.Controllers;

/// <summary>
/// 报税管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TaxController : ControllerBase
{
    /// <summary>
    /// 获取税务档案列表
    /// </summary>
    [HttpGet("profiles")]
    public IActionResult GetTaxProfiles([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        return Ok(new
        {
            items = new[]
            {
                new { id = Guid.NewGuid(), employeeId = Guid.NewGuid(), employeeName = "张三", taxNumber = "310101199001011234", taxType = "resident", deductions = 5000m, isActive = true },
                new { id = Guid.NewGuid(), employeeId = Guid.NewGuid(), employeeName = "李四", taxNumber = "310101199002022345", taxType = "resident", deductions = 6000m, isActive = true }
            },
            total = 2,
            page,
            pageSize
        });
    }

    /// <summary>
    /// 创建税务档案
    /// </summary>
    [HttpPost("profiles")]
    public IActionResult CreateTaxProfile([FromBody] object data)
    {
        return Ok(new { id = Guid.NewGuid(), message = "创建成功" });
    }

    /// <summary>
    /// 更新税务档案
    /// </summary>
    [HttpPut("profiles/{id:guid}")]
    public IActionResult UpdateTaxProfile(Guid id, [FromBody] object data)
    {
        return Ok(new { message = "更新成功" });
    }

    /// <summary>
    /// 删除税务档案
    /// </summary>
    [HttpDelete("profiles/{id:guid}")]
    public IActionResult DeleteTaxProfile(Guid id)
    {
        return Ok(new { message = "删除成功" });
    }

    /// <summary>
    /// 获取报税记录列表
    /// </summary>
    [HttpGet("records")]
    public IActionResult GetTaxRecords([FromQuery] int year = 2024, [FromQuery] int? month = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        return Ok(new
        {
            items = new[]
            {
                new { id = Guid.NewGuid(), employeeId = Guid.NewGuid(), employeeName = "张三", year = 2024, month = 12, grossIncome = 25000m, deductions = 7000m, taxableIncome = 18000m, taxAmount = 1080m, status = "filed" },
                new { id = Guid.NewGuid(), employeeId = Guid.NewGuid(), employeeName = "李四", year = 2024, month = 12, grossIncome = 20000m, deductions = 6000m, taxableIncome = 14000m, taxAmount = 630m, status = "pending" }
            },
            total = 2,
            page,
            pageSize
        });
    }

    /// <summary>
    /// 导出报税记录
    /// </summary>
    [HttpGet("records/export")]
    public IActionResult ExportTaxRecords([FromQuery] int year, [FromQuery] int? month)
    {
        return Ok(new { message = "导出功能待实现" });
    }

    /// <summary>
    /// 获取税务设置
    /// </summary>
    [HttpGet("settings")]
    public IActionResult GetTaxSettings()
    {
        return Ok(new
        {
            taxYear = 2024,
            basicDeduction = 5000,
            socialInsuranceRate = 0.105,
            housingFundRate = 0.12,
            childEducation = 1000,
            continuingEducation = 400,
            housingLoanInterest = 1000,
            housingRent = 1500,
            elderlySupport = 2000,
            infantCare = 1000
        });
    }

    /// <summary>
    /// 更新税务设置
    /// </summary>
    [HttpPut("settings")]
    public IActionResult UpdateTaxSettings([FromBody] object data)
    {
        return Ok(new { message = "保存成功" });
    }
}
