using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using kuujinbo.Mvc.NET.Filters;

namespace kuujinbo.Mvc.NET.Examples
{
    [ExcludeFromCodeCoverage]
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            FilterProviders.Providers.Add(XsrfFilter.Get());
        }
    }
}