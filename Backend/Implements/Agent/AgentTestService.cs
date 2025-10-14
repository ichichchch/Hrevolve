using Bots;

using Interfaces.Admin;

namespace Implements.Agent
{
    public class AgentTestService : IAgentTest
    {
        public async Task<string> Init()
        {

            var running = new Running();


            return await running.Init();
            
            
        }
    }
}
