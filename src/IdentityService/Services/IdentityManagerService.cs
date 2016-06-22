using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using IdentityAdmin.Core;
using IdentityService.Domain;
using IdentityService.Domain.Entitys;
using IdentityService.Domain.Models;
using IdentityService.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.ServiceFabric.AspNetCore.Hosting;
using Microsoft.ServiceFabric.Services.Runtime;
using Nature.Core;
using Nature.Core.ObjectMapping;
using Nature.Core.ServiceFabric;

namespace IdentityService.Services
{
    public class IdentityManagerService : NatureStatelessService, IIdentityManagerService
    {

        private UserManager UserManager => RequestProvider.GetService<UserManager>();
        private IObjectMapper ObjectMapper => RequestProvider.GetService<IObjectMapper>();
        private readonly AspNetCoreCommunicationContext _communicationContext;


        public IdentityManagerService(StatelessServiceContext serviceContext, AspNetCoreCommunicationContext communicationContext) : base(serviceContext)
        {
            _communicationContext = communicationContext;
        }

        public async Task<CallbackResult<string>> CreateUserAsync(CreateUserModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var validateResult = await new UserModelValidation().ValidateAsync(model);

            if (!validateResult.IsValid)
            {
                return new CallbackResult<string>(validateResult.Errors.Select(x=>x.ErrorMessage).ToArray());
            }

            using (BegionScope())
            {
                var user = ObjectMapper.Map<User>(model);

                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return new CallbackResult<string>(user.Id);
                }

                return new CallbackResult<string>(result.Errors.Select(x => x.Description).ToArray());
            }
        }

        public Task<IdentityResult> DeleteUserAsync(string key)
        {
            throw new NotImplementedException();
        }

    }
}
