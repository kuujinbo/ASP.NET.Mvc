using System.Web.Mvc;
using kuujinbo.Mvc.NET.Properties;

namespace kuujinbo.Mvc.NET.HtmlHelpers
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
            ScriptManagerHelper.AddInlineScript(helper, JavaScriptBlock, SCRIPT_KEY);

            return new MvcHtmlString("");
        }
    }
}
