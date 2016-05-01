using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityServer3.Core.Configuration;
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
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseIISPlatformHandler();

            app.UseStaticFiles();

            app.UseMvc();

            app.UseDatabaseErrorPage();
            app.UseDeveloperExceptionPage();


            app.Map("/core", config =>
            {
                var factory = new IdentityServerServiceFactory()
                    .UseAspNetCoreIdentity(config)
                    .UseInMemoryClients(Clients.Get())
                    .UseInMemoryScopes(Scopes.Get());

                var idsrvOptions = new IdentityServerOptions
                {
                    Factory = factory,
                    //.UseInMemoryUsers(Users.Get())
                    //.UseInMemoryClients(Clients.Get())
                    //.UseInMemoryScopes(Scopes.Get()),

                    RequireSsl = false
                };
                config.UseIdentityServer(idsrvOptions);
            });

            await SampleData.InitializeIdentityDatabaseAsync(app.ApplicationServices);

        }

        public static class SampleData
        {
            public static async Task InitializeIdentityDatabaseAsync(IServiceProvider serviceProvider)
            {
                using (var db = serviceProvider.GetRequiredService<IdentityContext>())
                {
                        await db.Database.EnsureDeletedAsync();

                    if (await db.Database.EnsureCreatedAsync())
                    {
                        await CreateAdminUser(serviceProvider);
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
