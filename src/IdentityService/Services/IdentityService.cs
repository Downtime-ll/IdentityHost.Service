using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using IdentityServer3.AspNetCore.Identity;
using IdentityServer3.AspNetIdentity;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using IdentityService.Models;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.OptionsModel;

namespace IdentityService.Services
{
    public static class UserServiceExtensions
    {

        public static void AddIdentityUserService<T>(this IServiceCollection serviceCollection) where T :class ,IUserService 
        {
            serviceCollection.AddScoped<IUserService, IdentityUserService>();
            serviceCollection.AddIdentity<User, Role>();
            serviceCollection.AddScoped<UserManager<User>, UserManager>();
            serviceCollection.AddScoped<RoleManager<Role>, RoleManager>();
            serviceCollection.AddScoped<IUserStore<User>, IdentityUserStore>();
            serviceCollection.AddScoped<IRoleStore<Role>, RoleStore>();
        }

        public static IdentityServerServiceFactory UseAspNetCoreIdentity(this IdentityServerServiceFactory factory, IApplicationBuilder appBuilder)
        {
            factory.UserService = new Registration<IUserService>(_ => appBuilder.ApplicationServices.GetService<IUserService>());
            return factory;
        }
    }
    public class IdentityUserService : AspNetIdentityUserService<User, string>
    {
        public IdentityUserService(UserManager<User> userManager, IOptions<AspNetCoreIdentityOptions> options) : base(userManager, options)
        {
        }
    }
}
