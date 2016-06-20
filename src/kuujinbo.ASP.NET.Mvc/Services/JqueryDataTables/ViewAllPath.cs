using System;
using System.Linq;
using System.Web;

namespace kuujinbo.ASP.NET.Mvc.Services.JqueryDataTables
{
    /// <summary>
    /// sadly, need extra code for unit tests
    /// </summary>
    public class HttpRuntimeWrapper
    {
        public virtual string AppDomainAppVirtualPath
        {
            get { return HttpRuntime.AppDomainAppVirtualPath; }
        }
    }

    public class ViewAllPath
    {
        public const string SEGMENT = "view-all";
        /// <summary>
        /// source code for HttpRuntime.AppDomainAppVirtualPath returns '/'
        /// or a virtual path that does not end with a slash
        /// </summary>
        private string _appDomainAppVirtualPath;

        public ViewAllPath(HttpRuntimeWrapper httpRuntimeWrapper)
        {
            _appDomainAppVirtualPath = httpRuntimeWrapper.AppDomainAppVirtualPath;
        }

        public static bool All(Uri url)
        {
            return url != null
                ? url.Segments.Last().Equals(SEGMENT, StringComparison.OrdinalIgnoreCase)
                : false;
        }

        public static string MakeUrl(string controllerName = null)
        {
            // var basePath = _appDomainAppVirtualPath.TrimEnd(new char[] { '/' });
            var basePath = HttpRuntime.AppDomainAppVirtualPath.TrimEnd(new char[] { '/' });

            return controllerName == null
                ? string.Format("{0}/{1}", basePath, SEGMENT)
                : string.Format("{0}/{1}/{2}", basePath, controllerName, SEGMENT);
        }
    }
}