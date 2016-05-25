using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using AutoMapper;
using IdentityAdmin;
using Modules = IdentityServer3.Core.Models;
using Entitys = IdentityServer3.EntityFrameworkCore.Entities;
using IdentityServer3.EntityFrameworkCore.DbContexts;
using IdentityServer3.EntityFrameworkCore.Extensions;

namespace IdentityService.Migrations
{
    public static class SeedDataExtensions
    {
        public static async void SendData(this IServiceProvider serviceProvider)
        {
            using (var service = serviceProvider.GetService<IServiceScopeFactory>().CreateScope())
            {

                using (var db = service.ServiceProvider.GetRequiredService<NullDbContext>())
                {
                    //db.Database.EnsureDeleted();
                    if (db.Database.EnsureCreated())
                    {
                        using (var db1 = service.ServiceProvider.GetRequiredService<IdentityContext>())
                        {
                            await db1.Database.MigrateAsync();
                            await CreateAdminUser(serviceProvider);
                        }

                        using (var db3 = service.ServiceProvider.GetRequiredService<IdentityClientConfigurationContext>())
                        {
                            await db3.Database.MigrateAsync();
                            await CreateClients(db3);
                        }

                        using (var db2 = service.ServiceProvider.GetRequiredService<IdentityScopeConfigurationContext>())
                        {
                            await db2.Database.MigrateAsync();
                            await CreateScopes(db2);
                        }

                        using (var db5 = service.ServiceProvider.GetRequiredService<IdentityOperationalContext>())
                        {
                            await db5.Database.MigrateAsync();
                        }
                    }
                }
            }
        }

        private static async Task CreateAdminUser(IServiceProvider serviceProvider)
        {
            const string adminRole = "admin";

            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new Role() { Name = adminRole });
                await roleManager.CreateAsync(new Role() { Name = "IdentityServerAdmin" });
                await roleManager.CreateAsync(new Role() { Name = "IdentityManagerAdmin" });
            }

            var user = await userManager.FindByNameAsync("admin");
            if (user == null)
            {
                user = new User { UserName = "admin", Email = "admin@mail.com", FirstName = "Lyu" };
                await userManager.CreateAsync(user, "admin");
                await userManager.AddToRoleAsync(user, adminRole);
                await userManager.AddToRoleAsync(user, "IdentityServerAdmin");
                await userManager.AddToRoleAsync(user, "IdentityManagerAdmin");
                await userManager.AddClaimAsync(user, new Claim("ManageStore", "Allowed"));
            }
        }

        private static async Task CreateClients(ClientConfigurationContext dbContext)
        {
            dbContext.Clients.Add(new IdentityServer3.EntityFrameworkCore.Entities.Client()
            {
                ClientId = "idmgr_and_idadmin",
                ClientName = "IdentityManager and IdentityServer.Admin",
                Enabled = true,
                Flow = IdentityServer3.Core.Models.Flows.Implicit,
                RequireConsent = false,
                IdentityTokenLifetime = 3600,
                AccessTokenLifetime = 3600,
                EnableLocalLogin = true,
                RedirectUris = new List<Entitys.ClientRedirectUri>()
                {
                    new Entitys.ClientRedirectUri() {Uri = "http://localhost:58319/"},
                    new Entitys.ClientRedirectUri() {Uri = "http://localhost:58319/adm"},
                },
                IdentityProviderRestrictions = new List<Entitys.ClientIdPRestriction>()
                {
                    new Entitys.ClientIdPRestriction() { Provider = IdentityServer3.Core.Constants.PrimaryAuthenticationType}
                },
                AllowedScopes =new List<Entitys.ClientScope>() {
                    new Entitys.ClientScope() {Scope =IdentityServer3.Core.Constants.StandardScopes.OpenId },
                    new Entitys.ClientScope() {Scope =IdentityServer3.Core.Constants.StandardScopes.Profile },
                    new Entitys.ClientScope() {Scope =IdentityManager.Constants.IdMgrScope },
                    new Entitys.ClientScope() {Scope =IdentityAdmin.Constants.IdAdminScope },
              }
            });

            await dbContext.SaveChangesAsync();
        }

        private static async Task CreateScopes(ScopeConfigurationContext dbContext)
        {
            
            dbContext.Scopes.Add(Modules.StandardScopes.OpenId.ToEntity());
            dbContext.Scopes.Add(Modules.StandardScopes.Email.ToEntity());
            dbContext.Scopes.Add(Modules.StandardScopes.Profile.ToEntity());
            dbContext.Scopes.Add(Modules.StandardScopes.OfflineAccess.ToEntity());

            dbContext.Scopes.Add(new IdentityServer3.EntityFrameworkCore.Entities.Scope()
            {
                Name = IdentityManager.Constants.IdMgrScope,
                DisplayName = "IdentityManager",
                Description = "Authorization for IdentityManager",
                Type = 0,
                Enabled = true,
                ScopeClaims = new List<Entitys.ScopeClaim>()
                {
                    new Entitys.ScopeClaim() {Name = Constants.ClaimTypes.Name},
                    new Entitys.ScopeClaim() {Name = Constants.ClaimTypes.Role},
                }
            });

            dbContext.Scopes.Add(new Entitys.Scope()
            {
                Name = IdentityAdmin.Constants.IdAdminScope,
                DisplayName = "IdentityServer.Admin",
                Description = "Authorization for IdentityServer.Admin",
                Type = 0,
                Enabled = true,
                ScopeClaims = new List<Entitys.ScopeClaim>()
                {
                    new Entitys.ScopeClaim() {Name = Constants.ClaimTypes.Name},
                    new Entitys.ScopeClaim() {Name = Constants.ClaimTypes.Role},
                }
            });

            await dbContext.SaveChangesAsync();
        }
    }
}
