using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer3.AspNetCore.Identity;
using IdentityServer3.AspNetIdentity;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using IdentityService.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Owin;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

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
            serviceCollection.AddSingleton(typeof (IServiceCollection), serviceCollection);
        }

        public static IdentityServerServiceFactory UseAspNetCoreIdentity(this IdentityServerServiceFactory factory, 
            IApplicationBuilder appBuilder)
        {
            factory.Register(new Registration<IServiceScopeFactory>(appBuilder.ApplicationServices.GetService<IServiceScopeFactory>()));

            factory.UserService =
                new Registration<IUserService>(resolver =>
                        resolver.Resolve<IServiceScopeFactory>()
                            .CreateScope()
                            .ServiceProvider.GetService<IUserService>());
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
