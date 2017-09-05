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
                "~/Content/themes/base/jquery-ui.css",
                "~/Content/octicons/octicons.css",
                "~/Content/site.css"
            ));

            bundles.Add(new ScriptBundle("~/bundles/jquery")
            //    .Include("~/Scripts/lib/bootstrap.js")
                .Include("~/Scripts/lib/jquery-{version}.js")
                .Include("~/Scripts/lib/jquery-ui-{version}.js")
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
        }
    }
}