using System.Diagnostics.CodeAnalysis;
using System.Web.Optimization;

namespace kuujinbo.ASP.NET.Mvc
{
    [ExcludeFromCodeCoverage]
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css")
                .Include("~/Content/bootstrap.css",
                "~/Scripts/lib/plupload/jquery.plupload.queue/css/jquery.plupload.queue.css",
                "~/Content/dataTables.bootstrap.css",
                "~/Content/themes/base/jquery-ui.css",
                "~/Content/octicons/octicons.css",
                "~/Content/site.css",
                "~/Content/jQueryDataTables.css"
            ));

            bundles.Add(new ScriptBundle("~/bundles/jquery")
                .Include("~/Scripts/lib/jquery-{version}.js")
            );

            bundles.Add(new ScriptBundle("~/bundles/bootstrap")
                .Include("~/Scripts/lib/bootstrap.js")
            );

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

            bundles.Add(new ScriptBundle("~/bundles/jQueryDataTables").Include(
                "~/Scripts/lib/DataTables/jquery.dataTables.min.js",
                "~/Scripts/lib/DataTables/dataTables.bootstrap.min.js",
                "~/Scripts/lib/jquery-ui-1.12.0.min.js",
                "~/Scripts/jQueryAjax/FileSaver.js",
                "~/Scripts/jQueryAjax/jquery.binarytransport.js",
                "~/Scripts/jQueryAjax/jquery-binary.js",
                "~/Scripts/jQueryDataTable/configTable.js",
                "~/Scripts/jQueryDataTable/Table.js"
            ));
        }
    }
}