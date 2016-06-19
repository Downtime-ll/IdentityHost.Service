using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityService.Domain.Entitys;
using IdentityService.Domain.Models;
using Nature.Core;

namespace IdentityService.Domain
{
    public interface IIdentityManagerService
    {
        Task<CallbackResult<string>> CreateUserAsync(CreateUserModel user);

        Task<IdentityResult> DeleteUserAsync(string key);



    }
}
