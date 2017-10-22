using System.Web.Mvc;
using kuujinbo.Mvc.NET.Helpers;
using kuujinbo.Mvc.NET.Properties;

namespace kuujinbo.Mvc.NET.HtmlHelpers
{
    public static class SessionTerminatorHelper
    {
        public const string ShowClientModalKey = "__showTerminatorModal"; 

        /// <summary>
        /// Ensure that JavaScript block only added once.
        /// </summary>
        public static readonly string ScriptKey = typeof(SessionTerminatorHelper).ToString();

        /// <summary>
        /// SessionTerminator.js. See https://github.com/kuujinbo/Mvc.NET/blob/master/src/kuujinbo.Mvc.NET/JavaScript/SessionTerminator.js
        /// </summary>
        public static readonly string JavaScriptBlock = Resources.SessionTerminator_min;

        /// <summary>
        /// JavaScript SessionTerminator snippet rendered to HTML output.
        /// </summary>
        public const string InitFormat = "new SessionTerminator().init({0}, '{1}');";

        /// <summary>
        /// JavaScript snippet rendered to HTML output that shows a modal 
        /// message explaining the inactivity timeout and application logout.
        /// </summary>
        public const string ShowLogout = "new SessionTerminator().showLogoutMessage();";

        /// <summary>
        /// Complete JavaScript rendered to HTML output. See:
        /// <see cref="kuujinbo.Mvc.NET.SessionTerminator.Logout" />.
        /// <see cref="kuujinbo.Mvc.NET.Attributes.SessionTerminatorIgnoreAttribute" />.
        /// </summary>
        public static MvcHtmlString TerminateSession(
            this HtmlHelper helper, int timeout, string url)
        {
            var tempData = helper.ViewContext.Controller.TempData;
            if (tempData[SessionTerminator.IgnoreSessionTimeout] == null)
            {
                url = helper.ViewContext.HttpContext.Request.AppendQueryString(
                    url, ShowClientModalKey
                );

                ScriptManagerHelper.AddInlineScript(
                    helper,
                    JavaScriptBlock + string.Format(InitFormat, timeout, url),
                    ScriptKey
                );
            }
            else if (tempData[SessionTerminator.SessionTimedOut] != null)
            {
                ScriptManagerHelper.AddInlineScript(
                    helper,
                    JavaScriptBlock + ShowLogout,
                    ScriptKey
                );
            }

            return new MvcHtmlString("");
        }
    }
}