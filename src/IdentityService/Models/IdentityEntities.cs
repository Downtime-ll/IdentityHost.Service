using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using IdentityServer3.EntityFrameworkCore.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using IdentityServer3.EntityFrameworkCore.Entities;
using IdentityService.Domain;
using IdentityService.Domain.Entitys;
using IdentityService.Services;

namespace IdentityService.Models
{
 

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

    /// <summary>
    /// 空数据库，用于创建数据库用
    /// </summary>
    public class NullDbContext : DbContext
    {
        public NullDbContext(DbContextOptions<NullDbContext> options)
            : base(options)
        { }
    }

    public class IdentityScopeConfigurationContext : ScopeConfigurationContext
    {
        public IdentityScopeConfigurationContext(DbContextOptions<IdentityScopeConfigurationContext> options) : base(options)
        {
        }
    }

    public class IdentityClientConfigurationContext : ClientConfigurationContext
    {
        public IdentityClientConfigurationContext(DbContextOptions<IdentityClientConfigurationContext> options) : base(options)
        {
        }
    }

    public class IdentityOperationalContext : OperationalContext
    {
        public IdentityOperationalContext(DbContextOptions<IdentityOperationalContext> options) : base(options)
        {
        }
    }

    public class IdentityAdminManagerService : IdentityAdminCoreManager<Client, Scope, IdentityScopeConfigurationContext, IdentityClientConfigurationContext>
    {
        public IdentityAdminManagerService(IdentityScopeConfigurationContext scopeContext, IdentityClientConfigurationContext clientContext) : base(scopeContext, clientContext)
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