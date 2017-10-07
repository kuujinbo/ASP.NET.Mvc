using kuujinbo.Mvc.NET;
using System.Web.Mvc;

namespace kuujinbo.Mvc.NET.Examples.Controllers
{
    public class DodCacController : Controller
    {
        private ICacUser _cacUser;
        private IClientCertificate _clientCertificate;

        public DodCacController(ICacUser cacInfo, IClientCertificate cert)
        {
            _cacUser = cacInfo;
            _clientCertificate = cert;
        }

        // GET: CacInfo
        public ActionResult Index()
        {
            var cacInfo = _cacUser.Create(_clientCertificate.GetCertificate(Request));

            return !string.IsNullOrWhiteSpace(cacInfo.Email)
                ? View(cacInfo) : View();
        }
    }
}