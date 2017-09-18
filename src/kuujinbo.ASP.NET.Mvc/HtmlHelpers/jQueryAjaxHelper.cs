using System.Web.Mvc;
using kuujinbo.ASP.NET.Mvc.Properties;

namespace kuujinbo.ASP.NET.Mvc.HtmlHelpers
{
    public static class jQueryAjaxHelper
    {
        /// <summary>
        /// Flag when extension called multiple times per view to ensure that
        /// JavaScript block only added once.
        /// </summary>
        public static readonly string SCRIPT_KEY = typeof(jQueryAjaxHelper).ToString();

        public static readonly string JavaScriptBlock = Resources.jQueryAjaxHelper_min;

        public static MvcHtmlString jQueryAjax(this HtmlHelper helper)
        {
            ScriptManagerHelper.AddViewScript(helper, JavaScriptBlock, SCRIPT_KEY);

            return new MvcHtmlString("");
        }
    }
}