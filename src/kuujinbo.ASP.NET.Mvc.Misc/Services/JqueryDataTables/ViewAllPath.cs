using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kuujinbo.ASP.NET.Mvc.Misc.Services.JqueryDataTables
{
    public class ViewAllPath
    {
        public const string SEGMENT = "view-all";

        public static bool All(Uri url)
        {
            return url != null
                ? url.Segments.Last().Equals(SEGMENT, StringComparison.OrdinalIgnoreCase)
                : false;
        }

        public static string MakeUrl(string controllerName = null)
        {
            var basePath = HttpRuntime.AppDomainAppVirtualPath
                .TrimEnd(new char[] { '/' });

            return controllerName == null
                ? string.Format("{0}/{1}", basePath, SEGMENT)
                : string.Format("{0}/{1}/{2}", basePath, controllerName, SEGMENT);
        }
    }
}