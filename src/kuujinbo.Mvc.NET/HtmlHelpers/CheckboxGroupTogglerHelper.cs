using System.Web.Mvc;
using kuujinbo.Mvc.NET.Properties;

namespace kuujinbo.Mvc.NET.HtmlHelpers
{
    public static class CheckboxGroupTogglerHelper
    {
        /// <summary>
        /// Flag when extension called multiple times per view to ensure that
        /// JavaScript block only added once.
        /// </summary>
        public static readonly string SCRIPT_KEY = typeof(CheckboxGroupTogglerHelper).ToString();

        public static readonly string JavaScriptBlock = Resources.CheckboxGroupToggler_min;

        public const string JAVASCRIPT_FORMAT =
@"new CheckboxGroupToggler('{0}').addToggleElement({1});";

        public static MvcHtmlString CheckboxGroupToggler(
            this HtmlHelper helper,
            string cssSelector,
            bool addAfter = false)
        {
            ScriptManagerHelper.AddInlineScript(helper, JavaScriptBlock, SCRIPT_KEY);
            ScriptManagerHelper.AddInlineScript(
                helper, 
                string.Format(JAVASCRIPT_FORMAT, cssSelector, addAfter.ToString().ToLower())
            );

            return new MvcHtmlString("");
        }
    }
}