namespace OpenVidSum.Services
{
    public interface ISummarizerService
    {
        Task<List<string>> Summarize(string videoLink);
    }
}
