using System.Web.Mvc;
using kuujinbo.Mvc.NET.Properties;

namespace kuujinbo.Mvc.NET.HtmlHelpers
{
    public static class SessionTerminatorHelper
    {
        /// <summary>
        /// Flag when extension called multiple times per view to ensure that
        /// JavaScript block only added once.
        /// </summary>
        public static readonly string SCRIPT_KEY = typeof(SessionTerminatorHelper).ToString();

        public static readonly string JavaScriptBlock = Resources.SessionTerminator_min;

        public const string InitFormat = "new SessionTerminator().init({0}, '{1}');";

        public const string ShowLogout = "new SessionTerminator().showLogoutMessage();";

        public static MvcHtmlString TerminateSession(
            this HtmlHelper helper, int timeout, string url)
        {
            ScriptManagerHelper.AddViewScript(helper, JavaScriptBlock, SCRIPT_KEY);

            var tempData = helper.ViewContext.Controller.TempData;

            if (tempData[SessionTerminator.IGNORE_SESSION_TIMEOUT] == null)
            {
                ScriptManagerHelper.AddViewScript(
                    helper, string.Format(InitFormat, timeout, url)
                );
            }
            else if (tempData[SessionTerminator.SESSION_TIMED_OUT] != null)
            {
                ScriptManagerHelper.AddViewScript(
                    helper, ShowLogout
                );
            }

            return new MvcHtmlString("");
        }
    }
}