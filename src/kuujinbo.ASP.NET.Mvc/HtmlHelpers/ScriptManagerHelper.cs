using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace kuujinbo.ASP.NET.Mvc.HtmlHelpers
{
    /// <summary>
    /// Solve the **HUGE** ASP.NET MVC script manager omission.
    /// </summary>
    public static class ScriptManagerHelper
    {
        public const string BAD_SCRIPT_PARAM = "script";

        public static readonly string ITEMS_KEY = typeof(ScriptManagerHelper).ToString();

        /// <summary>
        /// Add JavaScript to any view
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="script"></param>
        /// <param name="scriptKey"></param>
        /// <remarks>
        /// </remarks>
        public static MvcHtmlString AddViewScript(this HtmlHelper helper, 
            string script, 
            string scriptKey = null)
        {
            if (string.IsNullOrWhiteSpace(script)) throw new ArgumentException(BAD_SCRIPT_PARAM);

            if (string.IsNullOrWhiteSpace(scriptKey))
            {
                if (helper.ViewContext.HttpContext.Items[ITEMS_KEY] != null)
                {
                    ((IList<string>)helper.ViewContext.HttpContext.Items[ITEMS_KEY]).Add(script);
                }
                else
                {
                    helper.ViewContext.HttpContext.Items[ITEMS_KEY] = new List<string>() { script };
                }
            }
            else
            {
                if (!helper.ViewContext.HttpContext.Items.Contains(scriptKey))
                {
                    helper.ViewContext.HttpContext.Items[scriptKey] = null;
                    if (helper.ViewContext.HttpContext.Items[ITEMS_KEY] != null)
                    {
                        ((IList<string>)helper.ViewContext.HttpContext.Items[ITEMS_KEY]).Add(script);
                    }
                    else
                    {
                        helper.ViewContext.HttpContext.Items[ITEMS_KEY] = new List<string>() { script };
                    }
                }
            }

            return new MvcHtmlString(String.Empty);
        }

        /// <summary>
        /// Render JavaScript from all AddViewScript() calls.
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

                foreach (var script in scripts)
                {
                    helper.ViewContext.Writer.WriteLine(script);
                }
            }

            return new MvcHtmlString(String.Empty);
        }
    }
}