using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;

namespace OpenVidSum.Services
{
    public class ChatGPTService : IChatGPTService
    {
        private readonly IConfiguration _configuration;
        public ChatGPTService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<string>> GetChatGPTResponse(string[] prompts)
        {
            var apiKey = _configuration.GetSection("AppSettings:GChatAPIKEY").Value;
            List<string> responses = new List<string>();
            OpenAIAPI api = new OpenAIAPI(new APIAuthentication(apiKey));

            ChatRequest chatRequest = new ChatRequest()
            {
                Model = Model.ChatGPTTurbo,
                Temperature = 0.1,
                MaxTokens = 1000,
            };

            List<ChatMessage> messages = new List<ChatMessage>()
            {
                new ChatMessage(ChatMessageRole.System, "Summarize concisely the transcript from YouTube video.")
            };

            foreach (string prompt in prompts)
            {
                messages.Add(new ChatMessage(ChatMessageRole.User, prompt));
            }

            chatRequest.Messages = messages;

            var result = await api.Chat.CreateChatCompletionAsync(chatRequest);

            foreach (var choice in result.Choices)
            {
                if (choice.Message.Role == ChatMessageRole.Assistant)
                {
                    responses.Add(choice.Message.Content);
                }
            }

            return responses;
        }
    }
}
