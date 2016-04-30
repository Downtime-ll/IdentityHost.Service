

using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace IdentityHost.Service.Models
{
    [Table("User")]
    public class User : IdentityUser 
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class Role : IdentityRole { }

    public class Context : IdentityDbContext<User, Role, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
    {
        public Context(string connString)
            : base(connString)
        {
        }
    }

    public class UserStore : UserStore<User, Role, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
    {
        public UserStore(Context ctx) 
            : base(ctx)
        {
        }
    }

    public class UserManager : UserManager<User, string>
    {
        public UserManager(UserStore store)
            : base(store)
        {
            this.RegisterTwoFactorProvider("sms", new Sms());
        }
    }

    public class Sms : Microsoft.AspNet.Identity.PhoneNumberTokenProvider<User, string>
    {
        public override Task<bool> ValidateAsync(string purpose, string token, UserManager<User, string> manager, User user)
        {
            // just hard coding to validate any 2fa token
            return Task.FromResult(true);
            //return base.ValidateAsync(purpose, token, manager, user);
        }
    }

    public class RoleStore : RoleStore<Role>
    {
        public RoleStore(Context ctx)
            : base(ctx)
        {
        }
    }

    public class RoleManager : RoleManager<Role>
    {
        public RoleManager(RoleStore store)
            : base(store)
        {
        }
    }


}