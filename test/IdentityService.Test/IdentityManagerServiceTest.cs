using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using IdentityService.Domain;
using IdentityService.Domain.Models;
using IdentityService.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Nature.Core.ObjectMapping;
using Xunit;
using IdentityService.Services;

namespace IdentityService.Test
{
    public class IdentityManagerServiceTest
    {
        private readonly IServiceProvider _serviceProvider;
        public IdentityManagerServiceTest()
        {
            var serviceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

            var services = new ServiceCollection();
            services.AddOptions();
            services.AddDbContext<IdentityContext>(b => 
                        b.UseInMemoryDatabase().UseInternalServiceProvider(serviceProvider));

            services.AddIdentityUserService<IdentityUserService>();
            services.AddScoped<IIdentityManagerService, Services.IdentityService>();
            services.AddLogging();
            services.AddOptions();
            services.AddCoreServices();
            _serviceProvider = services.BuildServiceProvider();

            IApplicationBuilder builder = new ApplicationBuilder(_serviceProvider);
            builder.UseAtuoMapper();
        }

        [Fact]
        public async void CreateUser_ReturnValidatorError()
        {
            var service = _serviceProvider.GetService<IIdentityManagerService>();

            var createUser = new CreateUserModel()
            {
            };
            var result = await service.CreateUserAsync(createUser);

            Assert.False(result.IsSuccess);
            Assert.Equal(result.Errors.Count(), 3);
        }

        [Fact]
        public async void CreateUser_Success()
        {
            var service = _serviceProvider.GetService<IIdentityManagerService>();

            var createUser = new CreateUserModel()
            {
                Name = "张三",
                Email = "test@test.com",
                PhoneNumber = "1399999999",
                UserName = "test",
                Password = "admin",
                ConfirmPassword = "admin"
            };
            var result = await service.CreateUserAsync(createUser);

            var dbContext =_serviceProvider.GetService<IdentityContext>();
            var data = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == result.Result);

            Assert.True(result.IsSuccess);
            Assert.Equal(dbContext.Users.Count(), 1);
            Assert.Equal(data.Name, createUser.Name);
            Assert.Equal(data.Email, createUser.Email);
            Assert.Equal(data.PhoneNumber, createUser.PhoneNumber);
            Assert.Equal(data.UserName, createUser.UserName);
        }
    }
}
