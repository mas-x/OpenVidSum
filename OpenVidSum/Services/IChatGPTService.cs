namespace OpenVidSum.Services
{
    public interface IChatGPTService
    {
        Task<List<string>> GetChatGPTResponse(string prompt);
    }
}
