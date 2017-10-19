using System;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace kuujinbo.Mvc.NET.Attributes
{
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
            /* !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
             * System.Web.Helpers.AntiForgeryConfig.GetAntiForgeryCookieName()
             * (System.Web.WebPages.dll) appends garbage to the cookie name 
             * that is dependent on the web application root.
             * !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            */
            string cookieToken = cookie != null ? cookie.Value : null;

            AntiForgery.Validate(
                cookieToken, request.Headers["__RequestVerificationToken"]
                //                            ^^^^^^^^^^^^^^^^^^^^^^^^^^    
                // HtmlHelper.AntiForgeryToken() **ALYWAS** sets the
                // hidden form field name to above; would be nice if M$ 
                // gave programmatic access so no hard-coded value....
            );
        }
    }
}