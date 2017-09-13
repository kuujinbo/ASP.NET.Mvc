using kuujinbo.ASP.NET.Mvc.Helpers;
using kuujinbo.ASP.NET.Mvc.Properties;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace kuujinbo.ASP.NET.Mvc.HtmlHelpers
{
    public static class jQueryAutoCompleteHelper
    {
        /// <summary>
        /// Flag when extension called multiple times per view to ensure that
        /// JavaScript block only added once.
        /// </summary>
        public static readonly string VIEW_DATA = typeof(jQueryAutoCompleteHelper).ToString();

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
            if (htmlAttributes != null)
            {
                tagBuilder.MergeAttributes<string, object>(
                    HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)
                );
            }
            tagBuilder.MergeAttribute("id", cssIdSelector, true);
            tagBuilder.MergeAttribute("search-url", searchUrl, true);
            tagBuilder.MergeAttribute("min-search-length", minSearchLength.ToString(), true);

            var html = new StringBuilder(tagBuilder.ToString(), 4096);

            if (!helper.ViewData.ContainsKey(VIEW_DATA))
            {
                html.Append(JavaScriptBlock);
                helper.ViewData.Add(VIEW_DATA, true);
            }

            return new MvcHtmlString(html.ToString());
        }
    }
}