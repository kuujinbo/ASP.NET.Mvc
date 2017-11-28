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
    public static class JQueryAutoCompleteHelper
    {
        /// <summary>
        /// HTML input element attributes
        /// </summary>
        public const string IdAttribute = "id";
        public const string NameAttribute = "name";
        public const string UrlAttribute = "search-url";
        public const string MinLengthAttribute = "min-search-length";

        /// <summary>
        /// Flag when extension called multiple times per view to ensure that
        /// JavaScript block only added once.
        /// </summary>
        public static readonly string ScriptKey = typeof(JQueryAutoCompleteHelper).ToString();

        /// <summary>
        /// The JavaScript rendered to the browser.
        /// </summary>
        public static readonly string JavaScriptBlock = Resources.JQueryAutoComplete_min;

        public static MvcHtmlString JQueryAutoComplete(
            this HtmlHelper helper
            , string idAttribute
            , string searchUrl
            , int minSearchLength = 1
            , object htmlAttributes = null)
        {
            JQueryXhrHelper.JQueryXhr(helper);

            var tagBuilder = new TagBuilder("input");
            tagBuilder.MergeAttributes<string, object>(
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)
            );
            tagBuilder.MergeAttribute(IdAttribute, idAttribute, true);
            tagBuilder.MergeAttribute(NameAttribute, idAttribute, true);
            tagBuilder.MergeAttribute(UrlAttribute, searchUrl, true);
            tagBuilder.MergeAttribute(MinLengthAttribute, minSearchLength.ToString(), true);

            ScriptManagerHelper.AddInlineScript(helper, JavaScriptBlock, ScriptKey);

            return new MvcHtmlString(tagBuilder.ToString());
        }
    }
}