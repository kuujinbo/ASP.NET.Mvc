using System.Web.Mvc;
using System.Web.Routing;

namespace kuujinbo.ASP.NET.Mvc.Attributes
{
    public sealed class DodBannerAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Default DOD Banner controller name
        /// </summary>
        public const string CONTROLLER_NAME = "NoticeAndConsent";
        /// <summary>
        /// Default DOD Banner controller action
        /// </summary>
        public const string CONTROLLER_ACTION = "Index";

        string _controllerName = CONTROLLER_NAME;
        /// <summary>
        /// DOD Banner controller name
        /// </summary>
        public string ControllerName
        {
            get { return _controllerName; }
            set { _controllerName = value; }
        }

        string _controllerActionName = CONTROLLER_ACTION;
        /// <summary>
        /// DOD Banner controller action
        /// </summary>
        public string ControllerActionName
        {
            get { return _controllerActionName; }
            set { _controllerActionName = value; }
        }

        /// <summary>
        /// Deny access to any application page without first acknowledging DOD banner.
        /// </summary>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var context = filterContext.HttpContext;
            var request = filterContext.HttpContext.Request;

            // force DOD banner acknowledgement
            if (request.Cookies[CookieFactory.DOD_NOTICE_CONSENT] == null)
            {
                // redirect if return URL exists
                if (context.Response.Cookies[CookieFactory.RETURN_URL] == null
                    || string.IsNullOrWhiteSpace(context.Response.Cookies[CookieFactory.RETURN_URL].Value))
                {
                    context.Response.SetCookie(
                        CookieFactory.Create(
                            CookieFactory.RETURN_URL,
                            request.Url.PathAndQuery,
                            secure: request.Url.Scheme.Equals("https")
                        )
                    );
                }
                // redirect to application home
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = ControllerName, action = ControllerActionName })
                );
            }
        }
    }
}