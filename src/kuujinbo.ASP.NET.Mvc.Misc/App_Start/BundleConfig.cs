using System.Web;
using System.Web.Optimization;

namespace kuujinbo.ASP.NET.Mvc.Misc
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css")
                .Include("~/Content/bootstrap.css",
                "~/Scripts/lib/plupload/jquery.plupload.queue/css/jquery.plupload.queue.css",
                "~/Content/jquery.dataTables.css",
                "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/jquery")
                .Include("~/Scripts/lib/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                 "~/Scripts/lib/angular.js",
                 "~/Scripts/app.js",
                 "~/Scripts/appConfig.js",
                 "~/Scripts/Directives/pluploadWrapper.js",
                 "~/Scripts/httpRequestInterceptorTest.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/plupload").Include(
                "~/Scripts/lib/plupload/plupload.full.min.js",
                "~/Scripts/lib/plupload/jquery.plupload.queue/jquery.plupload.queue.min.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/dataTables").Include(
                "~/Scripts/lib/DataTables/jquery.dataTables.min.js",
                "~/Scripts/DataTables/jQDataTablesIndex.js"
            ));

        }
    }
}
