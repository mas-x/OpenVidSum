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

        public async Task<List<string>> Summarize(string videoLink)
        {           
            if (string.IsNullOrEmpty(videoLink))
                return new List<string>();

            string transcriptText = ConvertTranscriptToText(_transcriptAPI.GetTranscript(GetVideoID(videoLink)));

            if (string.IsNullOrEmpty(transcriptText))
                return new List<string>();

            List<string> allResponses = new List<string>();
            foreach (string prompt in GeneratePrompts(transcriptText))
            {
                List<string> responses = await _chatGPTService.GetChatGPTResponse(new string[] { prompt });
                allResponses.AddRange(responses);
            }

            return allResponses;
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

        private string[] GeneratePrompts(string transcript)
        {
            List<string> prompts = new List<string>();
            string[] words = Regex.Split(transcript, @"\W|_");
            while (words.Length > 2000)
            {
                string prompt = string.Join(" ", words.Take(2000));
                prompts.Add(prompt);
                words = words.Skip(2000).ToArray();
            }

            prompts.Add(string.Join(" ", words));
            //prompts.Insert(0, string.Format("In the next {0} messages, I will send the transcript of YouTube, you have to summarize it concisely. ", prompts.Count));

            return prompts.ToArray();
        }
    }
}
