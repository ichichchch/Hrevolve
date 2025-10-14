namespace Bots
{
    public class Running
    {

      
        public async Task<string> Init()
        {

            string? apiKey = "sk-c176c0cb40dc44008e6c6ccf11a2ef37";

            string? modleId = "qwen3-max";

            var url = new Uri("https://dashscope.aliyuncs.com/compatible-mode/v1");


            //配置可兼容OpenAI API的服务
            var credential = new ApiKeyCredential(apiKey);
            var modelOptions = new OpenAIClientOptions { Endpoint = url };

            //Agent 名字和说明
            string? agentName = "Zerø";
            string? instructions = "你是一名资深HRM系统顾问，擅长将人力资源战略与系统能力相结合，精通招聘、绩效、薪酬、人才发展等核心模块的流程设计与系统落地，致力于通过数字化手段提升组织效能。";


            AIAgent agent = new OpenAIClient(credential, modelOptions)
                .GetChatClient(modleId)
                .CreateAIAgent(instructions,agentName);


            //对话
            var chatMessage = new UserChatMessage("请为我介绍下市面上主流的HRM系统");

            var chatCompletion = await agent.RunAsync([chatMessage]);

            return chatCompletion.Content[^1].Text;

        }






    }
}
