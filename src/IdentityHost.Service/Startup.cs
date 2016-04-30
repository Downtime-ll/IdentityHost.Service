using System.Web.Http;
using IdentityHost.Service.IdSvr;
using IdentityHost.Service.Services;
using IdentityManager.Configuration;
using IdentityServer3.Core.Configuration;
using Owin;

namespace IdentityHost.Service
{
    public static class Startup
    {
        // 此代码会配置 Web API。启动类指定为
        // WebApp.Start 方法中的类型参数。
        public static void ConfigureApp(IAppBuilder appBuilder)
        {
            // 配置自托管的 Web API。 
            HttpConfiguration config = new HttpConfiguration();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            appBuilder.UseWebApi(config);

            appBuilder.Map("/admin", adminApp =>
            {
                var factory = new IdentityManagerServiceFactory();
                factory.ConfigureSimpleIdentityManagerService("AspId");

                adminApp.UseIdentityManager(new IdentityManagerOptions()
                {
                    Factory = factory,
                    SecurityConfiguration = {RequireSsl = false}
                });
            });

            appBuilder.Map("/core", core =>
            {
                var idSvrFactory = Factory.Configure();
                idSvrFactory.ConfigureUserService("AspId");

                var options = new IdentityServerOptions
                {
                    SiteName = "IdentityServer3 - AspNetIdentity 2FA",
                    RequireSsl = false,
                    //SigningCertificate = Certificate.Get(),
                    Factory = idSvrFactory,
                };

                core.UseIdentityServer(options);
            });
        }
    }
}
