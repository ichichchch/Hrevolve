namespace Hrevolve.Web.Controllers;

/// <summary>
/// 报销管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExpensesController : ControllerBase
{
    /// <summary>
    /// 获取报销类型列表
    /// </summary>
    [HttpGet("types")]
    public IActionResult GetExpenseTypes()
    {
        return Ok(new[]
        {
            new { id = Guid.NewGuid(), code = "TRAVEL", name = "差旅费", maxAmount = 5000m, requiresReceipt = true, isActive = true, description = "出差产生的交通、住宿费用" },
            new { id = Guid.NewGuid(), code = "MEAL", name = "餐饮费", maxAmount = 500m, requiresReceipt = true, isActive = true, description = "业务招待餐费" },
            new { id = Guid.NewGuid(), code = "OFFICE", name = "办公用品", maxAmount = 1000m, requiresReceipt = true, isActive = true, description = "办公用品采购" }
        });
    }

    /// <summary>
    /// 创建报销类型
    /// </summary>
    [HttpPost("types")]
    public IActionResult CreateExpenseType([FromBody] object data)
    {
        return Ok(new { id = Guid.NewGuid(), message = "创建成功" });
    }

    /// <summary>
    /// 更新报销类型
    /// </summary>
    [HttpPut("types/{id:guid}")]
    public IActionResult UpdateExpenseType(Guid id, [FromBody] object data)
    {
        return Ok(new { message = "更新成功" });
    }

    /// <summary>
    /// 删除报销类型
    /// </summary>
    [HttpDelete("types/{id:guid}")]
    public IActionResult DeleteExpenseType(Guid id)
    {
        return Ok(new { message = "删除成功" });
    }

    /// <summary>
    /// 获取报销申请列表
    /// </summary>
    [HttpGet("requests")]
    public IActionResult GetExpenseRequests([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        return Ok(new
        {
            items = new[]
            {
                new { id = Guid.NewGuid(), title = "北京出差报销", employeeId = Guid.NewGuid(), employeeName = "张三", expenseTypeId = Guid.NewGuid(), totalAmount = 3500m, status = "pending", createdAt = DateTime.Now.AddDays(-2), description = "客户拜访出差费用" },
                new { id = Guid.NewGuid(), title = "办公用品采购", employeeId = Guid.NewGuid(), employeeName = "李四", expenseTypeId = Guid.NewGuid(), totalAmount = 680m, status = "approved", createdAt = DateTime.Now.AddDays(-5), description = "部门办公用品" }
            },
            total = 2,
            page,
            pageSize
        });
    }

    /// <summary>
    /// 创建报销申请
    /// </summary>
    [HttpPost("requests")]
    public IActionResult CreateExpenseRequest([FromBody] object data)
    {
        return Ok(new { id = Guid.NewGuid(), message = "提交成功" });
    }

    /// <summary>
    /// 审批报销申请
    /// </summary>
    [HttpPost("requests/{id:guid}/approve")]
    public IActionResult ApproveExpenseRequest(Guid id, [FromBody] object data)
    {
        return Ok(new { message = "审批成功" });
    }
}
