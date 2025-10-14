using Interfaces.Admin;

namespace Hrevolve.Controllers.Agent
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController(IAgentTest _service) : ControllerBase
    {
        [HttpPost]
        public async Task<string> Test()
        {
           return await _service.Init();   
        }
    }
}
