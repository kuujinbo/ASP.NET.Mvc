using System.Text;
using System.Web.Mvc;
using kuujinbo.ASP.NET.Mvc.Properties;

namespace kuujinbo.ASP.NET.Mvc.HtmlHelpers
{
    public static class SessionTerminatorHelper
    {
        /// <summary>
        /// Flag when extension called multiple times per view to ensure that
        /// JavaScript block only added once.
        /// </summary>
        public static readonly string SCRIPT_KEY = typeof(SessionTerminatorHelper).ToString();

        public static readonly string JavaScriptBlock = Resources.SessionTerminator_min;

        public const string InitFormat = @"<script type='text/javascript'>
new SessionTerminator().init({0}, '{1}');
</script>";

        public const string ShowLogout = @"<script type='text/javascript'>
new SessionTerminator().showLogoutMessage();
</script>";

        public static MvcHtmlString TerminateSession(
            this HtmlHelper helper, int timeout, string url)
        {
            ScriptManagerHelper.AddViewScript(helper, JavaScriptBlock, SCRIPT_KEY);

            var tempData = helper.ViewContext.Controller.TempData;

            if (tempData[SessionTerminator.IGNORE_SESSION_TIMEOUT] == null)
            {
                return new MvcHtmlString(string.Format(InitFormat, timeout, url));
            }
            else if (tempData[SessionTerminator.SESSION_TIMED_OUT] != null)
            {
                return new MvcHtmlString(ShowLogout);
            }
            else { return new MvcHtmlString(""); }
        }
    }
}
