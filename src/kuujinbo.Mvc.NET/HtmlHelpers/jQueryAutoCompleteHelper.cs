using System.Text;
using System.Web.Mvc;
using kuujinbo.Mvc.NET.Properties;

namespace kuujinbo.Mvc.NET.HtmlHelpers
{
    /// <summary>
    /// Simple jQuery autocomplete helper. Caller is responsible for:
    /// [1] Hooking up server-side JSON response with collection of custom 
    ///     objects that **MUST** include 'label' and 'value' properties.
    /// [2] Hooking up client-side JavaScript to process server response.
    /// </summary>
    public static class jQueryAutoCompleteHelper
    {
        /// <summary>
        /// HTML input element attributes
        /// </summary>
        public const string ID_ATTR = "id";
        public const string URL_ATTR = "search-url";
        public const string MIN_LEN_ATTR = "min-search-length";

        /// <summary>
        /// Flag when extension called multiple times per view to ensure that
        /// JavaScript block only added once.
        /// </summary>
        public static readonly string SCRIPT_KEY = typeof(jQueryAutoCompleteHelper).ToString();

        public static readonly string JavaScriptBlock = Resources.jQueryAutoComplete;

        public static MvcHtmlString jQueryAutoComplete(
            this HtmlHelper helper
            , string cssIdSelector
            , string searchUrl
            , int minSearchLength = 1
            , object htmlAttributes = null)
        {
            JQueryXhrHelper.JQueryXhr(helper);

            var tagBuilder = new TagBuilder("input");
            tagBuilder.MergeAttributes<string, object>(
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)
            );
            tagBuilder.MergeAttribute(ID_ATTR, cssIdSelector, true);
            tagBuilder.MergeAttribute(URL_ATTR, searchUrl, true);
            tagBuilder.MergeAttribute(MIN_LEN_ATTR, minSearchLength.ToString(), true);

            ScriptManagerHelper.AddInlineScript(helper, JavaScriptBlock, SCRIPT_KEY);

            return new MvcHtmlString(tagBuilder.ToString());
        }
    }
}