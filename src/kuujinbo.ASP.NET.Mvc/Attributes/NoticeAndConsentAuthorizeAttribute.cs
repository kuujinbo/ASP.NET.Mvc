using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace kuujinbo.ASP.NET.Mvc.Attributes
{
    [AttributeUsage(
        AttributeTargets.Method | AttributeTargets.Class,
        AllowMultiple = false,
        Inherited = true)
    ]
    public sealed class NoticeAndConsentAuthorizeAttribute : AuthorizeAttribute
    {
        public NoticeAndConsentAuthorizeAttribute(string controllerName, string actionName)
        {
            ControllerName = controllerName;
            ActionName = actionName;
        }

        /// <summary>
        /// Notice/Consent Acknowledgement controller name
        /// </summary>
        public string ControllerName { get; private set; }

        /// <summary>
        /// Notice/Consent Acknowledgement action name
        /// DOD Banner controller action
        /// </summary>
        public string ActionName { get; private set; } 

        /// <summary>
        /// Deny access to any application page without user acknowledgment.
        /// </summary>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var context = filterContext.HttpContext;
            var request = filterContext.HttpContext.Request;

            // force acknowledgement
            if (request.Cookies[CookieFactory.NOTICE_AND_CONSENT] == null)
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
                    new RouteValueDictionary(new { controller = ControllerName, action = ActionName })
                );
            }
        }
    }
}