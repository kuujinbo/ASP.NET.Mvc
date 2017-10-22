using System.Diagnostics.CodeAnalysis;
using System.Web.Optimization;

namespace kuujinbo.Mvc.NET.Examples
{
    [ExcludeFromCodeCoverage]
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css")
                .Include("~/Content/bootstrap.css",
                "~/Content/themes/base/jquery-ui.css",
                "~/Content/octicons/octicons.css",
                "~/Content/site.css"
            ));

            bundles.Add(new ScriptBundle("~/bundles/jquery")
                .Include("~/Scripts/lib/jquery-{version}.js")
                .Include("~/Scripts/lib/jquery-ui-{version}.js")
            );
        }
    }
}