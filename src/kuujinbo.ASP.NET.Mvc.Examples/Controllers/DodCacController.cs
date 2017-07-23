using kuujinbo.ASP.NET.Mvc;
using System.Web.Mvc;

namespace kuujinbo.ASP.NET.Mvc.Examples.Controllers
{
    public class DodCacController : Controller
    {
        private IDodCac _dodCac;
        private IClientCertificate _clientCertificate;

        public DodCacController(IDodCac cacInfo, IClientCertificate cert)
        {
            _dodCac = cacInfo;
            _clientCertificate = cert;
        }

        // GET: CacInfo
        public ActionResult Index()
        {
            var cacInfo = _dodCac.Get(_clientCertificate.Get(Request));

            return !string.IsNullOrWhiteSpace(cacInfo.Email)
                ? View(cacInfo) : View();
        }
    }
}