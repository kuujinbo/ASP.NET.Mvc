using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kuujinbo.ASP.NET.Mvc.Misc.Helpers
{
    public static class MyData
    {
        public const string SEGMENT = "my-data";

        public static bool IsMyUrl(Uri url)
        {
            return url.Segments[url.Segments.Length - 1]
                .Equals(SEGMENT, StringComparison.OrdinalIgnoreCase);
        }

        public static string MakeMyUrl(string controllerName)
        {
            var basePath = HttpRuntime.AppDomainAppVirtualPath;

            return string.Format(
                "{0}{1}{2}/{3}",
                basePath,
                basePath.EndsWith("/") ? string.Empty : "/",
                controllerName,
                SEGMENT
            );
        }
    }
}