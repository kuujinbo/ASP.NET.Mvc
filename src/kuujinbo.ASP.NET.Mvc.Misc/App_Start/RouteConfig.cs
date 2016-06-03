using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using System.Web.Routing;

namespace kuujinbo.ASP.NET.Mvc.Misc
{
    [ExcludeFromCodeCoverage]
    public class RouteConfig
    {
        public const string ALPHA_NUMERIC = "^[a-zA-Z0-9]+$";

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                constraints: new { controller = ALPHA_NUMERIC, action = ALPHA_NUMERIC }
            );

            //routes.MapRoute(
            //    name: "MyData",
            //    url: "{controller}/" + MyData.SEGMENT,
            //    defaults: new { controller = "jQueryDataTables", action = "Index" }
            //);
            // MVC default
            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);
        }
    }
}
