using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MVCUseDotnetCoreDI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var services = new ServiceCollection();
            ConfigureService(services);

            ///  必須自己實作 IDependencyResolver
            var reslover = new DotNetCoreDIDependencyReslover(services.BuildServiceProvider());

            /// 將 Resolver 換成自己實作的
            DependencyResolver.SetResolver(reslover);
        }

        private void ConfigureService(ServiceCollection services)
        {
            /// 抓出所有的Controller 注入到 DotnetCore  的 DI 中
            var controllers = typeof(MvcApplication).Assembly.GetExportedTypes()
               .Where(r => !r.IsAbstract)
               .Where(r => typeof(IController).IsAssignableFrom(r))
               .Where(r => r.Name.EndsWith("Controller"));

            foreach (var ctrl in controllers)
            {
                services.AddTransient(ctrl);
            }

            /// 注入 HttpClinet
            services.AddHttpClient();

            /// 注入 Logger 機制
            services.AddLogging(bulider =>
            {
                bulider.AddDebug();
                bulider.SetMinimumLevel(LogLevel.Trace);
            });
        }
    }

    /// <summary>
    /// 自訂解析
    /// </summary>
    internal class DotNetCoreDIDependencyReslover : IDependencyResolver
    {
        private IServiceProvider serviceProvider;

        public DotNetCoreDIDependencyReslover(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public object GetService(Type serviceType)
        {
            return serviceProvider.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return serviceProvider.GetServices(serviceType);
        }
    }
}
