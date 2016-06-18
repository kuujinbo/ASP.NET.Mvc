using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kuujinbo.ASP.NET.Mvc.Controllers
{
    public class ReportParams
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

    public class ReportsController : Controller
    {
        public ActionResult Index()
        {
            System.Threading.Thread.Sleep(760);
            return PartialView();
        }

        [HttpPost]
        public ActionResult Index(ReportParams reportParams)
        {
            return File(
                System.IO.File.ReadAllBytes(Server.MapPath("~/app_data/hello-world.pdf")),
                "application/pdf",
                "hello.pdf"
            );
        }
    }
}