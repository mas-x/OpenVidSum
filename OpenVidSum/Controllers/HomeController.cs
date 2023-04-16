using Microsoft.AspNetCore.Mvc;
using OpenVidSum.Services;

namespace OpenVidSum.Controllers
{
    public class HomeController : Controller
    {
        ISummarizerService _summarizer;
        public HomeController(ISummarizerService summarizer)
        {
            _summarizer = summarizer;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ViewResult> Index(string videoLink)
        {
            ViewData["SummarizedText"] = await _summarizer.Summarize(videoLink);
            return View();
        }
    }
}
