using System.Web.Mvc;
using kuujinbo.ASP.NET.Mvc.Misc.Services;

namespace kuujinbo.ASP.NET.Mvc.Misc.Controllers
{
    [RequireHttps]
    public class CacInfoController : Controller
    {
        // GET: CacInfo
        public ActionResult Index()
        {
            var cacInfo = new CacInfo()
                .Get(Request.ClientCertificate.Certificate);
            return View(cacInfo);
        }
    }
}