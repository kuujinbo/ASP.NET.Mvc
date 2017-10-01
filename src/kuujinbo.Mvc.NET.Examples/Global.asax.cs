using System.Diagnostics.CodeAnalysis;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using kuujinbo.Mvc.NET.Examples.Controllers;
using FluentValidation.Mvc;

namespace kuujinbo.Mvc.NET.Examples
{
    [ExcludeFromCodeCoverage]
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // disable default MVC validators
            ModelValidatorProviders.Providers.Clear();
            FluentValidationModelValidatorProvider.Configure();

            InitAutofac();
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        private void InitAutofac()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<DodCac>().As<IDodCac>();
            builder.RegisterType<ClientCertificate>().As<IClientCertificate>();

            // register all controllers using assembly scanning
            // builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // register single controller
            builder.RegisterType<DodCacController>().InstancePerRequest();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}
