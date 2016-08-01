using kuujinbo.ASP.NET.Mvc.Services;
using System.Web.Mvc;

namespace kuujinbo.ASP.NET.Mvc.Controllers
{
    public class CacInfoController : Controller
    {
        private ICacInfo _cacInfo;
        private IClientCertificate _clientCertificate;

        public CacInfoController(ICacInfo cacInfo, IClientCertificate cert)
        {
            _cacInfo = cacInfo;
            _clientCertificate = cert;
        }

        // GET: CacInfo
        public ActionResult Index()
        {
            var cacInfo = _cacInfo.Get(_clientCertificate.Get(Request));

            return !string.IsNullOrWhiteSpace(cacInfo.Email)
                ? View(cacInfo) : View();
        }
    }
}