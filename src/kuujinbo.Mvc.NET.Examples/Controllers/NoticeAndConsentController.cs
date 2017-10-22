using System;
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using kuujinbo.Mvc.NET.Attributes;

namespace kuujinbo.Mvc.NET.Examples.Controllers
{
    public class NoticeAndConsentController : Controller
    {
        private readonly ISessionTerminator _sessionTerminator;
        public NoticeAndConsentController(ISessionTerminator terminator)
        {
            _sessionTerminator = terminator;
        }

        [ExcludeFromCodeCoverage]
        [SessionTerminatorIgnore]
        public ActionResult Index()
        {
            return View();
        }

        [ExcludeFromCodeCoverage]
        public ActionResult Accept()
        {
            HttpCookieFactory.SetCookie(
                Response,
                NoticeAndConsentAuthorizeAttribute.NoticeAndConsent,
                DateTime.Now.ToShortDateString()
            );

            return View("Accepted");
        }

        [ExcludeFromCodeCoverage, SessionTerminatorIgnore]
        public ActionResult Refuse()
        {
            return View();
        }

        [ExcludeFromCodeCoverage]
        [NoticeAndConsentAuthorize("NoticeAndConsent", "Index")]
        public ActionResult Accepted()
        {
            return View();
        }

        public ActionResult Logout()
        {
            _sessionTerminator.Logout(Request, Response, TempData);

            // **MUST** redirect
            return RedirectToAction("Index");
        }
    }
}