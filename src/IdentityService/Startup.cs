using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityAdmin.Configuration;
using IdentityAdmin.Core;
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
using IdentityServer3.EntityFrameworkCore.DbContexts;
using Microsoft.AspNetCore.Identity;
using System.IO;

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
                .AddDbContext<IdentityContext>(options => options.UseSqlServer(connectionString))
                .AddDbContext<ClientConfigurationContext>(o => o.UseSqlServer(connectionString))
                .AddDbContext<ScopeConfigurationContext>(o => o.UseSqlServer(connectionString))
                .AddDbContext<MyOperationalContext>(o => o.UseSqlServer(connectionString));
        }

        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole()
                .CreateLogger();


            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap = new Dictionary<string, string>();

            //app.UseJwtBearerAuthentication(options =>
            //{
            //    options.Authority = "http://192.168.1.111:5005/core";
            //    options.RequireHttpsMetadata = false;

            //    options.Audience = "http://192.168.1.111:5005/core/resources";
            //    options.AutomaticAuthenticate = true;
            //});

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

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

                        Owin.IdentityAdminAppBuilderExtensions.UseIdentityAdmin(builder,new IdentityAdminOptions
                        {
                            Factory = factory,
                            AdminSecurityConfiguration = {RequireSsl = false}
                        });

                        var appFunc =
                            builder.Build(typeof (Func<IDictionary<string, object>, Task>)) as
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
                    .RegisterClientStore<int, ClientConfigurationContext>()
                    .RegisterScopeStore<int, ScopeConfigurationContext>();
              

                var certFile = env.WebRootPath + $"{System.IO.Path.DirectorySeparatorChar}idsrv3test.pfx";
                var idsrvOptions = new IdentityServerOptions
                {
                    Factory = factory,
                    RequireSsl = false,
                    SigningCertificate = new X509Certificate2(certFile, "idsrv3test"),
                };
                config.UseIdentityServer3(idsrvOptions);
            });

            await SampleData.InitializeIdentityDatabaseAsync(app.ApplicationServices);

        }

        public static class SampleData
        {
            public static async Task InitializeIdentityDatabaseAsync(IServiceProvider serviceProvider)
            {
                using (var service = serviceProvider.GetService<IServiceScopeFactory>().CreateScope())
                {

                    using (var db = service.ServiceProvider.GetRequiredService<IdentityContext>())
                    {
                        await db.Database.EnsureDeletedAsync();

                        await db.Database.MigrateAsync();
                        {
                            await CreateAdminUser(serviceProvider);
                        }
                    }

                    using (var db = service.ServiceProvider.GetRequiredService<ClientConfigurationContext>())
                    {
                        await db.Database.MigrateAsync();
                    }

                    using (var db = service.ServiceProvider.GetRequiredService<ScopeConfigurationContext>())
                    {
                        await db.Database.MigrateAsync();
                    }
                }
            }

            /// <summary>
            /// Creates a store manager user who can manage the inventory.
            /// </summary>
            /// <param name="serviceProvider"></param>
            /// <returns></returns>
            private static async Task CreateAdminUser(IServiceProvider serviceProvider)
            {
                const string adminRole = "admin";

                var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
                var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
                if (!await roleManager.RoleExistsAsync(adminRole))
                {
                    await roleManager.CreateAsync(new Role() {Name= adminRole });
                }

                var user = await userManager.FindByNameAsync("admin");
                if (user == null)
                {
                    user = new User { UserName = "admin", };
                    await userManager.CreateAsync(user, "YouShouldChangeThisPassword1!");
                    await userManager.AddToRoleAsync(user, adminRole);
                    await userManager.AddClaimAsync(user, new Claim("ManageStore", "Allowed"));
                }
            }
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
