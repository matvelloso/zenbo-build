using Autofac;
using Autofac.Integration.WebApi;
using System;
using System.Configuration;
using System.Reflection;
using System.Web.Http;
using Zenbo.BotService.Services;

namespace Zenbo.BotService
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());         
           
            var config = GlobalConfiguration.Configuration;
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}