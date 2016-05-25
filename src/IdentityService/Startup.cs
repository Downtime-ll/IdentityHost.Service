using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;
using IdentityServer3.Core.Configuration;
using IdentityService.Models;
using IdentityService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;


using DataProtectionProviderDelegate = System.Func<string[], System.Tuple<System.Func<byte[], byte[]>, System.Func<byte[], byte[]>>>;
using DataProtectionTuple = System.Tuple<System.Func<byte[], byte[]>, System.Func<byte[], byte[]>>;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityAdmin.Configuration;
using IdentityAdmin.Core;
using IdentityServer3.Core.Resources;
using IdentityServer3.Core.Services;
using IdentityServer3.EntityFrameworkCore.DbContexts;
using IdentityService.Migrations;
using IdentityServer3.EntityFrameworkCore.Extensions;
using IdentityServer3.EntityFrameworkCore.Stores;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
//using Microsoft.Owin;
using Microsoft.Owin.Security;
using Owin;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;

namespace IdentityService
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddIdentityUserService<IdentityUserService>();

            string connectionString = Configuration["Data:DefaultConnection:ConnectionString"];
            services.AddEntityFramework()
                .AddDbContext<NullDbContext>(options => options.UseSqlServer(connectionString))
                .AddDbContext<IdentityContext>(options => options.UseSqlServer(connectionString))
                .AddDbContext<IdentityClientConfigurationContext>(o => o.UseSqlServer(connectionString))
                .AddDbContext<IdentityScopeConfigurationContext>(o => o.UseSqlServer(connectionString))
                
                .AddDbContext<IdentityOperationalContext>(o => o.UseSqlServer(connectionString));

            services.AddScoped<OperationalContext, IdentityOperationalContext>();
            services.AddScoped<ClientConfigurationContext, IdentityClientConfigurationContext>();
            services.AddScoped<ScopeConfigurationContext, IdentityScopeConfigurationContext>();

            services.AddScoped<IClientStore, ClientStore>();
        }

        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //Log.Logger = new LoggerConfiguration()
            //    .MinimumLevel.Debug()
            //    .WriteTo.LiterateConsole()
            //    .CreateLogger();

            Log.Logger = new LoggerConfiguration()
                              .WriteTo.Trace(outputTemplate: "{Timestamp} [{Level}] ({Name}){NewLine} {Message}{NewLine}{Exception}")
                              .CreateLogger();

            var sourceSwitch = new SourceSwitch("LoggingSample");
            sourceSwitch.Level = SourceLevels.All;
            //loggerFactory.AddTraceSource(sourceSwitch,
            //    new ConsoleTraceListener(false));
            loggerFactory.AddTraceSource(sourceSwitch,
                new EventLogTraceListener("Application"));


            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap = new Dictionary<string, string>();

            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationScheme = "Cookies",
                AutomaticAuthenticate = true,
            });



            app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions()
            {
                AuthenticationScheme = "Oidc",
                SignInScheme = "Cookies",
                RequireHttpsMetadata = false,
                //Authority = "https://localhost:44333/core",
                Authority = "http://localhost:58319/core",
                CallbackPath = "/",

                
                ClientId = "idmgr_and_idadmin",
                UseTokenLifetime = true,
                Scope = { "openid", "idmgr", "idAdmin" },
                ResponseType = "id_token"
               
            });

            //    app.UseJwtBearerAuthentication(new JwtBearerOptions()
            //{

            //    Authority = "http://localhost:58319/core",
            //    RequireHttpsMetadata = false,

            //    Audience = "http://localhost:58319/resources",
            //    AutomaticAuthenticate = true
            //});



            app.UseStaticFiles();
            app.UseMvc();


            app.UseDatabaseErrorPage();
            app.UseDeveloperExceptionPage();

           
            app.Map("/adm", adminApp =>
            {
                var factory = new IdentityAdminServiceFactory();
                factory.Register(new IdentityAdmin.Configuration.Registration<IServiceScopeFactory>(app.ApplicationServices.GetService<IServiceScopeFactory>()));

                factory.IdentityAdminService =
                    new IdentityAdmin.Configuration.Registration<IIdentityAdminService>(
                        resolver => resolver.Resolve<IServiceScopeFactory>().CreateScope().ServiceProvider.GetService<IdentityAdminManagerService>());
                adminApp.UseOwin(addToPipeline =>
                {
                    addToPipeline(next =>
                    {
                        var builder = new Microsoft.Owin.Builder.AppBuilder();
                        var provider =
                            app.ApplicationServices.GetRequiredService<Microsoft.AspNetCore.DataProtection.IDataProtectionProvider>();

                        builder.Properties["security.DataProtectionProvider"] =
                            new DataProtectionProviderDelegate(purposes =>
                            {
                                var dataProtection = provider.CreateProtector(String.Join(",", purposes));
                                return new DataProtectionTuple(dataProtection.Protect, dataProtection.Unprotect);
                            });

                        var options = new IdentityAdminOptions
                        {
                            Factory = factory,
                            AdminSecurityConfiguration = new AdminHostSecurityConfiguration() {RequireSsl = false,AdminRoleName = "IdentityServerAdmin", HostAuthenticationType = IdentityAdmin.Constants.CookieAuthenticationType }
                        };

                        Owin.IdentityAdminAppBuilderExtensions.UseIdentityAdmin(builder, options);

                        var appFunc =
                            builder.Build(typeof(Func<IDictionary<string, object>, Task>)) as
                                Func<IDictionary<string, object>, Task>;
                        return appFunc;
                    });

                });

            });

            app.Map("/core", config =>
            {
                var factory = new IdentityServerServiceFactory()
                    .UseAspNetCoreIdentity(config);


                factory.ConfigureEntityFramework(app.ApplicationServices)
                    .RegisterOperationalStores()
                    .RegisterClientStore<IdentityClientConfigurationContext>()
                    .RegisterScopeStore<IdentityScopeConfigurationContext>();
              

                var certFile = env.WebRootPath + $"{System.IO.Path.DirectorySeparatorChar}idsrv3test.pfx";
                var idsrvOptions = new IdentityServerOptions
                {
                    Factory = factory,
                    RequireSsl = false,
                    
                    SigningCertificate = new X509Certificate2(certFile, "idsrv3test"),
                };
                config.UseIdentityServer3(idsrvOptions);
            });

            //初始化数据
            app.ApplicationServices.SendData();
        }

        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
