using System.Web.Mvc;
using kuujinbo.ASP.NET.Mvc.Misc.Services;

namespace kuujinbo.ASP.NET.Mvc.Misc.Controllers
{
    public class CacInfoController : Controller
    {
        private ICacInfo _cacInfo;

        public CacInfoController(ICacInfo cacInfo)
        {
            _cacInfo = cacInfo;
        }

        // GET: CacInfo
        public ActionResult Index()
        {
            var cacInfo = _cacInfo.Get(Request.ClientCertificate.Certificate);

            return View(cacInfo);
        }
    }
}