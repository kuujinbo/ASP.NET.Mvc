using System.Text;
using System.Web.Mvc;
using kuujinbo.ASP.NET.Mvc.Properties;

namespace kuujinbo.ASP.NET.Mvc.HtmlHelpers
{
    public static class CheckboxGroupTogglerHelper
    {
        /// <summary>
        /// Flag when extension called multiple times per view to ensure that
        /// JavaScript block only added once.
        /// </summary>
        public static readonly string SCRIPT_KEY = typeof(CheckboxGroupTogglerHelper).ToString();

        public static readonly string JavaScriptBlock;
        static CheckboxGroupTogglerHelper()
        {
            var script = new StringBuilder("<script type='text/javascript'>", 4096);
            script.Append(Resources.CheckboxGroupToggler_min);
            script.Append("</script>");
            JavaScriptBlock = script.ToString();
        }

        public const string JAVASCRIPT_FORMAT =
@"<script type='text/javascript'>new CheckboxGroupToggler('{0}').addToggleElement({1});</script>";

        public static MvcHtmlString CheckboxGroupToggler(
            this HtmlHelper helper,
            string cssSelector,
            bool addAfter = false)
        {
            ScriptManagerHelper.AddViewScript(helper, JavaScriptBlock, SCRIPT_KEY);
            ScriptManagerHelper.AddViewScript(
                helper, 
                string.Format(JAVASCRIPT_FORMAT, cssSelector, addAfter.ToString().ToLower())
            );

            return new MvcHtmlString("");
        }
    }
}