using kuujinbo.ASP.NET.Mvc.Attributes;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kuujinbo.ASP.NET.Mvc
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class IgnoreXsrfFilterAttribute : Attribute { }

    public static class XsrfFilter
    {
        public static readonly string[] XsrfMethods = 
        { 
            HttpVerbs.Post.ToString(), HttpVerbs.Delete.ToString(), HttpVerbs.Put.ToString()
        };

        public static object GetFilter(HttpRequestBase request, object[] ignoreAttribute)
        {
            if (ignoreAttribute.Length > 0
                || !IsXsrfMethod(request.HttpMethod)) return null;

            if (request.IsAjaxRequest()) 
                return new ValidateJsonAntiForgeryTokenAttribute();
 
            return new ValidateAntiForgeryTokenAttribute();
        }

        public static bool IsXsrfMethod(string method)
        {
            return XsrfMethods.Contains(method, StringComparer.OrdinalIgnoreCase);
        }

        /*
         * HttpRequestBase.IsAjaxRequest() **REQUIRES**:
         * 
         * -- header name: "X-Requested-With"
         * -- header value: "XMLHttpRequest"
         * 
         * or will **ALWAYS** return false. some JavaScript framework(s) like 
         * AngularJS mess this up, and **MUST** be explictily configured:
         * https://github.com/angular/angular.js/commit/3a75b1124d062f64093a90b26630938558909e8d
         */
        public static ConditionalFilterProvider Get()
        {
            return new ConditionalFilterProvider(
                new Func<ControllerContext, ActionDescriptor, object>[]
                {
                    (c, a) => GetFilter(
                        c.HttpContext.Request, 
                        // GetCustomAttributes() return value is **NEVER** null
                        a.GetCustomAttributes(typeof(IgnoreXsrfFilterAttribute), false)
                    )
                }
            );
        }

    }
}