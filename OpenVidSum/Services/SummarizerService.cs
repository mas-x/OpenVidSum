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
                return "";

            string transcriptText = ConvertTranscriptToText(_transcriptAPI.GetTranscript(GetVideoID(videoLink)));

            if (string.IsNullOrEmpty(transcriptText))
                return "";

            List<string> allResponses = new List<string>();
            foreach (string prompt in GeneratePrompts(transcriptText))
            {
                List<string> response = await _chatGPTService.GetChatGPTResponse(prompt);
                allResponses.AddRange(response);
            }

            if (allResponses.Count == 0)
                return "";

            StringBuilder responseBuilder = new StringBuilder();
            foreach (string response in allResponses)
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

        private List<string> GeneratePrompts(string transcript)
        {
            List<string> prompts = new List<string>();
            string[] words = Regex.Split(transcript, @"\W|_");
            while (words.Length > 3000)
            {
                string prompt = $"Generate a summary for the following text {string.Join(" ", words.Take(3000))}";
                prompts.Add(prompt);
                words = words.Skip(3000).ToArray();
            }

            prompts.Add($"Generate a summary for the following text {string.Join(" ", words)}");
            return prompts;
        }
    }
}
