using System.Web.Mvc;
using kuujinbo.ASP.NET.Mvc.Properties;

namespace kuujinbo.ASP.NET.Mvc.HtmlHelpers
{
    public static class JQueryXhrHelper
    {
        /// <summary>
        /// Flag when extension called multiple times per view to ensure that
        /// JavaScript block only added once.
        /// </summary>
        public static readonly string SCRIPT_KEY = typeof(JQueryXhrHelper).ToString();

        public static readonly string JavaScriptBlock = Resources.JQueryXhr_min;

        public static MvcHtmlString JQueryXhr(this HtmlHelper helper)
        {
            ScriptManagerHelper.AddViewScript(helper, JavaScriptBlock, SCRIPT_KEY);

            return new MvcHtmlString("");
        }
    }
}
