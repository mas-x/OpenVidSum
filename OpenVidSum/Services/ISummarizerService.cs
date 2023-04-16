namespace OpenVidSum.Services
{
    public interface ISummarizerService
    {
        Task<string> Summarize(string videoLink);
    }
}
