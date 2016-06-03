using System.Web.Mvc;
using kuujinbo.ASP.NET.Mvc.Misc.Services;

namespace kuujinbo.ASP.NET.Mvc.Misc.Controllers
{
    public class CacInfoController : Controller
    {
        // GET: CacInfo
        public ActionResult Index()
        {
            var cacInfo = CacInfo.Get(Request.ClientCertificate.Certificate);
            return View(cacInfo);
        }
    }
}