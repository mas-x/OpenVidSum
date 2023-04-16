using OpenAI_API;

namespace OpenVidSum.Services
{
    public class ChatGPTService : IChatGPTService
    {
        private readonly IConfiguration _configuration;
        public ChatGPTService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<string>> GetChatGPTResponse(string prompt)
        {
            var apiKey = _configuration.GetSection("AppSettings:GChatAPIKEY").Value;
            var apiModel = _configuration.GetSection("AppSettings:Model").Value;
            List<string> rq = new List<string>();
            string rs = "";
            OpenAIAPI api = new OpenAIAPI(new APIAuthentication(apiKey));
            var completionRequest = new OpenAI_API.Completions.CompletionRequest()
            {
                Prompt = prompt,
                Model = apiModel,
                Temperature = 0.6,
                MaxTokens = 2000,
                TopP = 1.0,
                FrequencyPenalty = 0.25,
                PresencePenalty = 0.0,
            };

            var result = await api.Completions.CreateCompletionsAsync(completionRequest);
            foreach (var choice in result.Completions)
            {
                rs = choice.Text;
                rq.Add(choice.Text);
            }

            return rq;
        }
    }
}
