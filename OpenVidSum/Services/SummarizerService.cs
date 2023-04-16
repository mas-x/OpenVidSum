using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using YoutubeTranscriptApi;

namespace OpenVidSum.Services
{
    public class SummarizerService : ISummarizerService
    {
        YouTubeTranscriptApi _transcriptAPI = null;
        IChatGPTService _chatGPTService = null;
        public SummarizerService(IChatGPTService chatGPTService, YouTubeTranscriptApi transcriptAPI)
        {
            _transcriptAPI = transcriptAPI;
            _chatGPTService = chatGPTService;
        }

        public async Task<string> Summarize(string videoLink)
        {
            if (string.IsNullOrEmpty(videoLink))
                throw new BadHttpRequestException("Invalid Video Link.");

            string transcriptText = ConvertTranscriptToText(_transcriptAPI.GetTranscript(GetVideoID(videoLink)));

            if (string.IsNullOrEmpty(transcriptText))
                throw new BadHttpRequestException("Unable to generate video transcript.");

            if (transcriptText.Length > 3000)
                throw new BadHttpRequestException("Transcript length exceeds maximum token limit.");

            List<string> responses = await _chatGPTService.GetChatGPTResponse(GeneratePrompt(transcriptText));

            if (responses.Count == 0)
                return "";

            StringBuilder responseBuilder = new StringBuilder();
            foreach (string response in responses)
                responseBuilder.AppendLine(response);

            return responseBuilder.ToString();
        }

        private string ConvertTranscriptToText(IEnumerable<TranscriptItem> transcriptItems)
        {
            StringBuilder text = new StringBuilder();
            foreach (TranscriptItem item in transcriptItems)
            {
                text.AppendLine(item.Text);
            }

            return text.ToString();
        }

        private string GetVideoID(string videoLink)
        {
            Uri uri = new Uri(videoLink);
            NameValueCollection query = HttpUtility.ParseQueryString(uri.Query);

            if (query == null || query.Count == 0)
                return "";

            return query["v"];
        }

        private string GeneratePrompt(string transcript)
        {
            if (transcript.Length > 3000)
                return "";

            return $"Generate a short and concise summary for the following video transcript \n{transcript}";
        }
    }
}
