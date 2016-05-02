using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.InMemory;
using IdentityService.Configuration;
using IdentityService.Models;
using IdentityService.Services;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Data.Entity;
using Microsoft.Extensions.OptionsModel;
using Serilog;
using InMemoryClientStore = IdentityService.Services.InMemoryClientStore;

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

            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<IdentityContext>(options => options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));

        }

        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole()
                .CreateLogger();


            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap = new Dictionary<string, string>();

            app.UseJwtBearerAuthentication(options =>
            {
                options.Authority = "http://192.168.1.111:5005/core";
                options.RequireHttpsMetadata = false;

                options.Audience = "http://192.168.1.111:5005/core/resources";
                options.AutomaticAuthenticate = true;
            });

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseIISPlatformHandler();

            app.UseStaticFiles();

            app.UseMvc();


            app.UseDatabaseErrorPage();
            app.UseDeveloperExceptionPage();

            var clientid = Clients.Get()[0].ClientId;
            app.Map("/core", config =>
            {
                var clients = Clients.Get();
                var factory = new IdentityServerServiceFactory()
                    .UseAspNetCoreIdentity(config)
                    .UseInMemoryScopes(Scopes.Get());

                factory.Register(new Registration<IEnumerable<Client>>(clients));
                factory.ClientStore = new Registration<IClientStore>(typeof(InMemoryClientStore));
                factory.CorsPolicyService = new Registration<ICorsPolicyService>(new InMemoryCorsPolicyService(clients));

                var certFile = env.WebRootPath + $"{System.IO.Path.DirectorySeparatorChar}idsrv3test.pfx";
                var idsrvOptions = new IdentityServerOptions
                {
                    Factory = factory,
                    //.UseInMemoryUsers(Users.Get())
                    //.UseInMemoryClients(Clients.Get())
                    //.UseInMemoryScopes(Scopes.Get()),

                    RequireSsl = false,
                    SigningCertificate = new X509Certificate2(certFile, "idsrv3test"),
                };
                config.UseIdentityServer(idsrvOptions);
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

                        if (await db.Database.EnsureCreatedAsync())
                        {
                            await CreateAdminUser(serviceProvider);
                        }
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

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
