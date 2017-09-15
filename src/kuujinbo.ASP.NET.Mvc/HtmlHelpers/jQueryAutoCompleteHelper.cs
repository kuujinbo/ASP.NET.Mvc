using kuujinbo.ASP.NET.Mvc.Properties;
using System.Text;
using System.Web.Mvc;

namespace kuujinbo.ASP.NET.Mvc.HtmlHelpers
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

        public static readonly string JavaScriptBlock;

        static jQueryAutoCompleteHelper()
        {
            var script = new StringBuilder("<script type='text/javascript'>", 4096);
            script.AppendLine(Resources.jQueryAutoComplete);
            script.AppendLine("</script>");
            JavaScriptBlock = script.ToString();
        }

        public static MvcHtmlString jQueryAutoComplete(
            this HtmlHelper helper
            , string cssIdSelector
            , string searchUrl
            , int minSearchLength = 1
            , object htmlAttributes = null)
        {
            var tagBuilder = new TagBuilder("input");
            tagBuilder.MergeAttributes<string, object>(
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)
            );
            tagBuilder.MergeAttribute(ID_ATTR, cssIdSelector, true);
            tagBuilder.MergeAttribute(URL_ATTR, searchUrl, true);
            tagBuilder.MergeAttribute(MIN_LEN_ATTR, minSearchLength.ToString(), true);

            //if (!helper.ViewData.ContainsKey(VIEW_DATA))
            //{
            //    html.Append(JavaScriptBlock);
            //    helper.ViewData.Add(VIEW_DATA, true);
            //}

            ScriptManagerHelper.AddViewScript(helper, JavaScriptBlock);

            return new MvcHtmlString(tagBuilder.ToString());
        }
    }
}