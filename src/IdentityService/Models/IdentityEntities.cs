using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using Microsoft.Data.Entity;

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
        public IdentityContext(DbContextOptions options)
            : base(options)
        {
        }
    }

    public class IdentityUserStore : UserStore<User,Role, IdentityContext>
    {
        public IdentityUserStore(IdentityContext ctx, IdentityErrorDescriber describer = null)
            : base(ctx, describer)
        {
        }


        public override Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            //ThrowIfDisposed();
            return Context.Set<User>().FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName, cancellationToken);
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
            ILogger<UserManager<User>> logger,
            IHttpContextAccessor contextAccessor)
            : base(store,optionsAccessor,passwordHasher,userValidators,passwordValidators,keyNormalizer,errors,services,logger,contextAccessor)
        {
            //this.RegisterTwoFactorProvider("sms", new Sms());
        }

        /// <summary>
        /// Finds and returns a user, if any, who has the specified normalized user name.
        /// </summary>
        /// <param name="normalizedUserName">The normalized user name to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="userID"/> if it exists.
        /// </returns>
        public override Task<User> FindByNameAsync(string userName)
        {
            //ThrowIfDisposed();
            if (userName == null)
            {
                throw new ArgumentNullException("userName");
            }
            userName = NormalizeKey(userName);
            return Store.FindByNameAsync(userName, CancellationToken.None);
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
        private CancellationToken CancellationToken =>  CancellationToken.None;
        public RoleManager(IRoleStore<Role> store, 
            IEnumerable<IRoleValidator<Role>> roleValidators, 
            ILookupNormalizer keyNormalizer, 
            IdentityErrorDescriber errors, 
            ILogger<RoleManager<Role>> logger, 
            IHttpContextAccessor contextAccessor)
            : base(store,roleValidators,keyNormalizer,errors,logger,contextAccessor)
        {
        }
    }




}