using System.Diagnostics.CodeAnalysis;
using System.Web.Helpers;

namespace System.Web.Mvc
{
    [ExcludeFromCodeCoverage]
    /// <summary>
    /// Workaround to allow Antiforgery Token Validation via HTTP Header for AJAX requests
    /// Reference: http://haacked.com/archive/2011/10/10/preventing-csrf-with-ajax.aspx/
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Method | AttributeTargets.Class, 
        AllowMultiple = false, 
        Inherited = true)
    ]
    public sealed class ValidateJsonAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null) throw new ArgumentNullException("filterContext");

            var request = filterContext.HttpContext.Request;
            var cookie = request.Cookies[AntiForgeryConfig.CookieName];
            string cookieToken = cookie != null ? cookie.Value : null;

            AntiForgery.Validate(
                cookieToken, request.Headers[AntiForgeryConfig.CookieName]
            );
        }
    }
}