using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace kuujinbo.Mvc.NET.Attributes
{
    /// <summary>
    /// Application entry point that requires users to acknowledge a notice
    /// and consent statement.
    /// </summary>
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
        /// Flag user acknowledged application entry notice.
        /// </summary>
        public const string NoticeAndConsent = "_http-cookie-notice-and-consent";

        /// <summary>
        /// Redirect for bookmarked page.
        /// </summary>
        public const string ReturnUrl = "_http-cookie-return-url";

        /// <summary>
        /// Notice/Consent acknowledgement controller name
        /// </summary>
        public string ControllerName { get; private set; }

        /// <summary>
        /// Notice/Consent acknowledgement action name
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
            if (request.Cookies[NoticeAndConsent] == null)
            {
                // redirect if return URL exists
                if (context.Response.Cookies[ReturnUrl] == null
                    || string.IsNullOrWhiteSpace(context.Response.Cookies[ReturnUrl].Value))
                {
                    context.Response.SetCookie(
                        HttpCookieFactory.Create(
                            ReturnUrl,
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