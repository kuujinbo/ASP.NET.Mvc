using System.Web.Mvc;
using kuujinbo.Mvc.NET.Properties;

namespace kuujinbo.Mvc.NET.HtmlHelpers
{
    public static class JQueryConfirmHelper
    {
        /// <summary>
        /// Flag when extension called multiple times per view to ensure that
        /// JavaScript block only added once.
        /// </summary>
        public static readonly string ScriptKey = typeof(JQueryConfirmHelper).ToString();

        /// <summary>
        /// The JavaScript rendered to the browser
        /// </summary>
        public static readonly string JavaScriptBlock = Resources.JQueryConfirm;

        public static MvcHtmlString JQueryConfirm(this HtmlHelper helper)
        {
            ScriptManagerHelper.AddInlineScript(helper, JavaScriptBlock, ScriptKey);

            return new MvcHtmlString("");
        }
    }
}
