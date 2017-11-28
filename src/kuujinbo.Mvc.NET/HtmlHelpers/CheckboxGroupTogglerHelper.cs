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
        public static readonly string ScriptKey = typeof(CheckboxGroupTogglerHelper).ToString();

        /// <summary>
        /// The JavaScript rendered to the browser.
        /// </summary>
        public static readonly string JavaScriptBlock = Resources.CheckboxGroupToggler_min;

        public const string JavaScriptFormat =
@"new CheckboxGroupToggler('{0}').addToggleElement({1});";

        public static MvcHtmlString CheckboxGroupToggler(
            this HtmlHelper helper,
            string cssSelector,
            bool addAfter = false)
        {
            ScriptManagerHelper.AddInlineScript(helper, JavaScriptBlock, ScriptKey);
            ScriptManagerHelper.AddInlineScript(
                helper, 
                string.Format(JavaScriptFormat, cssSelector, addAfter.ToString().ToLower())
            );

            return new MvcHtmlString("");
        }
    }
}