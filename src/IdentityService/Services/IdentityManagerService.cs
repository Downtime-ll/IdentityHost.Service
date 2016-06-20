using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityAdmin.Core;
using IdentityService.Domain;
using IdentityService.Domain.Entitys;
using IdentityService.Domain.Models;
using IdentityService.Models;
using Nature.Core;
using Nature.Core.ObjectMapping;

namespace IdentityService.Services
{
    public class IdentityService : IIdentityManagerService
    {

        private readonly UserManager _userManager;
        private readonly IObjectMapper _objectMapper;

        public IdentityService(UserManager userManager, IObjectMapper objectMapper)
        {
            _userManager = userManager;
            _objectMapper = objectMapper;
        }

        public async Task<CallbackResult<string>> CreateUserAsync(CreateUserModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var validateResult = await new UserModelValidation().ValidateAsync(model);

            if (!validateResult.IsValid)
            {
                return new CallbackResult<string>(validateResult.Errors.Select(x=>x.ErrorMessage).ToArray());
            }

            var user = _objectMapper.Map<User>(model);

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                return new CallbackResult<string>(user.Id);
            }

            return new CallbackResult<string>(result.Errors.Select(x => x.Description).ToArray());
        }

        public Task<IdentityResult> DeleteUserAsync(string key)
        {
            throw new NotImplementedException();
        }
    }
}
