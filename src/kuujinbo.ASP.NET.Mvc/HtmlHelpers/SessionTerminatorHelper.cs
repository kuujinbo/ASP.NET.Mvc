using System.Text;
using System.Web.Mvc;
using kuujinbo.ASP.NET.Mvc.Properties;

namespace kuujinbo.ASP.NET.Mvc.HtmlHelpers
{
    public static class SessionTerminatorHelper
    {
        public static readonly string JavaScriptBlock;
        static SessionTerminatorHelper()
        {
            var script = new StringBuilder("<script type='text/javascript'>", 4096);
            script.AppendLine(Resources.SessionTerminator);
            script.AppendLine("</script>");
            JavaScriptBlock = script.ToString();
        }

        public const string InitFormat = @"<script type='text/javascript'>
new SessionTerminator().init({0}, '{1}');
</script>";

        public const string ShowLogout = @"<script type='text/javascript'>
new SessionTerminator().showLogoutMessage();
</script>";

        public static MvcHtmlString TerminateSession(
            this HtmlHelper html, int timeout, string url)
        {
            var tempData = html.ViewContext.Controller.TempData;

            if (tempData[SessionTerminator.IGNORE_SESSION_TIMEOUT] == null)
            {
                return new MvcHtmlString(
                    JavaScriptBlock 
                    + string.Format(InitFormat, timeout, url)
                );
            }
            else if (tempData[SessionTerminator.SESSION_TIMED_OUT] != null)
            {
                return new MvcHtmlString(JavaScriptBlock + ShowLogout);
            }
            else { return new MvcHtmlString(""); }
        }
    }
}
