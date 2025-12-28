namespace Hrevolve.Web.Controllers;

/// <summary>
/// 公司设置控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CompanyController : ControllerBase
{
    /// <summary>
    /// 获取公司信息
    /// </summary>
    [HttpGet("tenant")]
    public IActionResult GetTenant()
    {
        // 返回模拟数据
        return Ok(new
        {
            id = Guid.NewGuid(),
            code = "DEMO",
            name = "演示公司",
            industry = "科技",
            scale = "100-500人",
            address = "上海市浦东新区",
            phone = "021-12345678",
            email = "contact@demo.com",
            website = "https://demo.com",
            status = "Active"
        });
    }

    /// <summary>
    /// 更新公司信息
    /// </summary>
    [HttpPut("tenant")]
    public IActionResult UpdateTenant([FromBody] object data)
    {
        return Ok(new { message = "更新成功" });
    }

    /// <summary>
    /// 获取成本中心树
    /// </summary>
    [HttpGet("cost-centers/tree")]
    public IActionResult GetCostCenterTree()
    {
        return Ok(new[]
        {
            new { id = Guid.NewGuid(), code = "CC001", name = "总部", parentId = (Guid?)null, isActive = true, children = new[]
            {
                new { id = Guid.NewGuid(), code = "CC001-01", name = "研发中心", parentId = (Guid?)null, isActive = true, children = Array.Empty<object>() },
                new { id = Guid.NewGuid(), code = "CC001-02", name = "销售中心", parentId = (Guid?)null, isActive = true, children = Array.Empty<object>() }
            }}
        });
    }

    /// <summary>
    /// 创建成本中心
    /// </summary>
    [HttpPost("cost-centers")]
    public IActionResult CreateCostCenter([FromBody] object data)
    {
        return Ok(new { id = Guid.NewGuid(), message = "创建成功" });
    }

    /// <summary>
    /// 更新成本中心
    /// </summary>
    [HttpPut("cost-centers/{id:guid}")]
    public IActionResult UpdateCostCenter(Guid id, [FromBody] object data)
    {
        return Ok(new { message = "更新成功" });
    }

    /// <summary>
    /// 删除成本中心
    /// </summary>
    [HttpDelete("cost-centers/{id:guid}")]
    public IActionResult DeleteCostCenter(Guid id)
    {
        return Ok(new { message = "删除成功" });
    }

    /// <summary>
    /// 获取标签列表
    /// </summary>
    [HttpGet("tags")]
    public IActionResult GetTags()
    {
        return Ok(new[]
        {
            new { id = Guid.NewGuid(), name = "核心员工", color = "#D4AF37", category = "员工", isActive = true },
            new { id = Guid.NewGuid(), name = "高潜人才", color = "#3498DB", category = "员工", isActive = true },
            new { id = Guid.NewGuid(), name = "技术专家", color = "#2ECC71", category = "技能", isActive = true }
        });
    }

    /// <summary>
    /// 创建标签
    /// </summary>
    [HttpPost("tags")]
    public IActionResult CreateTag([FromBody] object data)
    {
        return Ok(new { id = Guid.NewGuid(), message = "创建成功" });
    }

    /// <summary>
    /// 更新标签
    /// </summary>
    [HttpPut("tags/{id:guid}")]
    public IActionResult UpdateTag(Guid id, [FromBody] object data)
    {
        return Ok(new { message = "更新成功" });
    }

    /// <summary>
    /// 删除标签
    /// </summary>
    [HttpDelete("tags/{id:guid}")]
    public IActionResult DeleteTag(Guid id)
    {
        return Ok(new { message = "删除成功" });
    }

    /// <summary>
    /// 获取打卡设备列表
    /// </summary>
    [HttpGet("clock-devices")]
    public IActionResult GetClockDevices()
    {
        return Ok(new[]
        {
            new { id = Guid.NewGuid(), name = "前台指纹机", serialNumber = "FP001", type = "fingerprint", location = "1楼前台", ipAddress = "192.168.1.100", status = "online", isActive = true, lastSyncTime = DateTime.Now.AddMinutes(-5) },
            new { id = Guid.NewGuid(), name = "人脸识别机", serialNumber = "FC001", type = "face", location = "2楼入口", ipAddress = "192.168.1.101", status = "online", isActive = true, lastSyncTime = DateTime.Now.AddMinutes(-3) }
        });
    }

    /// <summary>
    /// 创建打卡设备
    /// </summary>
    [HttpPost("clock-devices")]
    public IActionResult CreateClockDevice([FromBody] object data)
    {
        return Ok(new { id = Guid.NewGuid(), message = "创建成功" });
    }

    /// <summary>
    /// 更新打卡设备
    /// </summary>
    [HttpPut("clock-devices/{id:guid}")]
    public IActionResult UpdateClockDevice(Guid id, [FromBody] object data)
    {
        return Ok(new { message = "更新成功" });
    }

    /// <summary>
    /// 删除打卡设备
    /// </summary>
    [HttpDelete("clock-devices/{id:guid}")]
    public IActionResult DeleteClockDevice(Guid id)
    {
        return Ok(new { message = "删除成功" });
    }
}
