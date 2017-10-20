using System.Web.Mvc;

namespace kuujinbo.Mvc.NET.Examples.Controllers
{
    public class CacUserController : Controller
    {
        private IClientCertificate _clientCertificate;

        public CacUserController(IClientCertificate cert)
        {
            _clientCertificate = cert;
        }

        public ActionResult Index()
        {
            var cacUser = _clientCertificate.GetCacUser(Request, true);

            return !string.IsNullOrWhiteSpace(cacUser.Email) ? View(cacUser) : View();
        }
    }
}