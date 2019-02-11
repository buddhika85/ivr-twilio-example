using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Transcriber;

namespace TestMvc.Controllers
{
    public class HomeController : Controller
    {
        public Task<string> Index()
        {
            var transcriptProvider = new CustomTranscriptProvider();
            var transcript = transcriptProvider.GetTranscriptFromWavFile("https://api.twilio.com/2010-04-01/Accounts/ACdfe342ddbecf65b044a7e098180a75e3/Recordings/REec946b8507f6cfd70727b754aae881af");
            return transcript;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}