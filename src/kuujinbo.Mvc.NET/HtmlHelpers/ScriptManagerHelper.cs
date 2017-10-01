using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace kuujinbo.Mvc.NET.HtmlHelpers
{
    /// <summary>
    /// Solve the **HUGE** ASP.NET MVC script manager omission.
    /// </summary>
    public static class ScriptManagerHelper
    {
        public const string BAD_SCRIPT_PARAM = "script";

        public static readonly string ITEMS_KEY = typeof(ScriptManagerHelper).ToString();
        public static readonly string SRC_KEY = typeof(ScriptManagerHelper).ToString() + "_SRC";

        public static MvcHtmlString AddScriptSrc(this HtmlHelper helper, string src)
        {
            if (helper.ViewContext.HttpContext.Items[SRC_KEY] != null)
            {
                ((IList<string>)helper.ViewContext.HttpContext.Items[SRC_KEY]).Add(src);
            }
            else
            {
                helper.ViewContext.HttpContext.Items[SRC_KEY] = new List<string>() { src };
            }

            return new MvcHtmlString(String.Empty);
        }

        /// <summary>
        /// Add JavaScript to any view
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="script"></param>
        /// <param name="scriptKey"></param>
        /// <remarks>
        /// </remarks>
        public static MvcHtmlString AddInlineScript(this HtmlHelper helper, 
            string script, 
            string scriptKey = null)
        {
            if (string.IsNullOrWhiteSpace(script)) throw new ArgumentException(BAD_SCRIPT_PARAM);

            if (scriptKey != null)
            {
                if (helper.ViewContext.HttpContext.Items.Contains(scriptKey))
                {
                    return new MvcHtmlString(String.Empty);
                }
                helper.ViewContext.HttpContext.Items[scriptKey] = null;
            }

            if (helper.ViewContext.HttpContext.Items[ITEMS_KEY] != null)
            {
                ((IList<string>)helper.ViewContext.HttpContext.Items[ITEMS_KEY]).Add(script);
            }
            else
            {
                helper.ViewContext.HttpContext.Items[ITEMS_KEY] = new List<string>() { script };
            }

            return new MvcHtmlString(String.Empty);
        }

        /// <summary>
        /// Render JavaScript from all AddInlineScript() calls.
        /// Use the Helper in ~/Views/Shared/_Layout.cshtml
        /// </summary>
        /// <remarks>
        /// SAMPLE USAGE in 
        /// <!--
        /// @Scripts.Render("~/bundles/0")
        /// @Scripts.Render("~/bundles/1")
        /// @RenderSection("scripts", required: false)
        /// @Html.RenderViewScripts()
        /// </body>
        /// </html>
        /// -->
        /// </remarks>
        public static MvcHtmlString RenderViewScripts(this HtmlHelper helper)
        {
            if (helper.ViewContext.HttpContext.Items[ITEMS_KEY] != null)
            {
                var scripts = (IList<string>)helper.ViewContext.HttpContext.Items[ITEMS_KEY];

                helper.ViewContext.Writer.WriteLine(@"<script type='text/javascript'>");
                foreach (var script in scripts)
                {
                    helper.ViewContext.Writer.WriteLine(script);
                }
                helper.ViewContext.Writer.WriteLine("</script>");
            }

            if (helper.ViewContext.HttpContext.Items[SRC_KEY] != null)
            {
                var urls = (IList<string>)helper.ViewContext.HttpContext.Items[SRC_KEY];
                foreach (var url in urls)
                {
                    helper.ViewContext.Writer.WriteLine(string.Format(
                        "<script type='text/javascript' src='{0}'></script>", url
                    ));
                }
            }

            return new MvcHtmlString(String.Empty);
        }
    }
}