using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace kuujinbo.ASP.NET.Mvc.HtmlHelpers
{
    public static class ScriptManagerHelper
    {
        public static readonly string ITEMS_KEY = typeof(ScriptManagerHelper).ToString();

        public static MvcHtmlString AddViewScript(this HtmlHelper helper, 
            string script, 
            string scriptKey = null)
        {
            if (string.IsNullOrWhiteSpace(script)) throw new ArgumentException("script");

            if (helper.ViewContext.HttpContext.Items[ITEMS_KEY] != null)
            {
                ((List<string>)helper.ViewContext.HttpContext.Items[ITEMS_KEY]).Add(script);
            }
            else
            {
                helper.ViewContext.HttpContext.Items[ITEMS_KEY] = new List<string>() { script };
            }

            return new MvcHtmlString(String.Empty);
        }

        public static MvcHtmlString RenderViewScripts(this HtmlHelper helper)
        {
            if (helper.ViewContext.HttpContext.Items[ITEMS_KEY] != null)
            {
                var scripts = (List<string>)helper.ViewContext.HttpContext.Items[ITEMS_KEY];

                foreach (var script in scripts) helper.ViewContext.Writer.Write(script);
            }

            return new MvcHtmlString(String.Empty);
        }
    }
}