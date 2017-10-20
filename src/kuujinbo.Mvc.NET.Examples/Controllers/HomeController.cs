using kuujinbo.Mvc.NET.Attributes;
using kuujinbo.Mvc.NET.Examples.Models;
using kuujinbo.Mvc.NET.Filters;
using kuujinbo.Mvc.NET.IO;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Web.Mvc;

namespace kuujinbo.Mvc.NET.Examples.Controllers
{
    [ExcludeFromCodeCoverage]
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult PdfResult()
        {
            return new PdfResult(Server.MapPath("~/app_data/moby-dick.pdf"), 256);
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Post()
        {
            TempData["result"] = string.Format(
                "POST: <b>with</b> AntiForgeryToken. URL: {0}", Request.Url
            );
            return Redirect("~/");
        }

        [HttpPost]
        [IgnoreXsrfFilter]
        public ActionResult PostIgnoreXsrfFilter()
        {
            TempData["resultPostIgnoreXsrfFilter"] = string.Format(
                "POST: <b>ignore</b> AntiForgeryToken. URL: {0}", Request.Url
            );
            return Redirect("~/");
        }
        
        [ValidateJsonAntiForgeryToken]
        [HttpPost]
        public ActionResult JsonAntiForgery(TestModel testModel)
        {
            Thread.Sleep(2000);
            return new JsonResult
            {
                Data = string.Format("data: {0} HTTP response @{1}", testModel.Name, DateTime.Now)
            };
        }
    }
}