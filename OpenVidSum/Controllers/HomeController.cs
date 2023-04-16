using Microsoft.AspNetCore.Mvc;
using OpenVidSum.Models;
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
            List<string> responses = await _summarizer.Summarize(videoLink);

            ViewData["Response"] = new ResponseViewModel()
            {
                Responses = responses,
                VideoLink = videoLink
            };

            return View();
        }
    }
}
