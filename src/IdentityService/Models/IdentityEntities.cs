using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer3.Admin.EntityFramework7;
using IdentityServer3.Admin.EntityFramework7.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using IdentityServer3.EntityFrameworkCore.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace IdentityService.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class Role : IdentityRole { }

    public class IdentityContext : IdentityDbContext<User, Role, string>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options)
            : base(options)
        {
        }
    }

    public class IdentityUserStore : UserStore<User, Role, IdentityContext>
    {
        public IdentityUserStore(IdentityContext ctx, IdentityErrorDescriber describer = null)
            : base(ctx, describer)
        {
        }


        public override Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            cancellationToken = CancellationToken.None;
            //ThrowIfDisposed();
            return Context.Set<User>().FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName, cancellationToken);
        }
    }

    public class ClientConfigurationContext : ClientConfigurationContext<int>
    {
        public ClientConfigurationContext(DbContextOptions<ClientConfigurationContext> options)
            : base(options)
        { }
    }

    public class ScopeConfigurationContext : ScopeConfigurationContext<int>
    {
        public ScopeConfigurationContext(DbContextOptions<ScopeConfigurationContext> options)
            : base(options)
        { }
    }

    public class MyOperationalContext : OperationalContext
    {
        public MyOperationalContext(DbContextOptions<MyOperationalContext> options) : base(options)
        {
        }
    }

    public class IdentityAdminManagerService : IdentityAdminCoreManager<IdentityClient, IdentityScope, ScopeConfigurationContext, ClientConfigurationContext>
    {
        public IdentityAdminManagerService(ScopeConfigurationContext scopeContext, ClientConfigurationContext clientContext) : base(scopeContext, clientContext)
        {
        }
    }

    public class UserManager : UserManager<User>
    {
        public UserManager(IUserStore<User> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<User> passwordHasher,
            IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<User>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }
    }

    //public class Sms : Microsoft.AspNet.Identity.PhoneNumberTokenProvider<User, string>
    //{
    //    public override Task<bool> ValidateAsync(string purpose, string token, UserManager<User, string> manager, User user)
    //    {
    //        // just hard coding to validate any 2fa token
    //        return Task.FromResult(true);
    //        //return base.ValidateAsync(purpose, token, manager, user);
    //    }
    //}

    public class RoleStore : RoleStore<Role>
    {
        public RoleStore(IdentityContext ctx)
            : base(ctx)
        {
        }
    }

    public class RoleManager : RoleManager<Role>
    {
        private CancellationToken CancellationToken => CancellationToken.None;
        public RoleManager(IRoleStore<Role> store,
            IEnumerable<IRoleValidator<Role>> roleValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            ILogger<RoleManager<Role>> logger,
            IHttpContextAccessor contextAccessor)
            : base(store, roleValidators, keyNormalizer, errors, logger, contextAccessor)
        {
        }
    }
}