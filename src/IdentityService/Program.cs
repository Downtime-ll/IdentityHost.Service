using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IdentityService.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.ServiceFabric.AspNetCore.Hosting;
using Microsoft.ServiceFabric.Services.Runtime;

namespace IdentityService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var communicationContext = CreateAspNetCoreCommunicationContext();

            ServiceRuntime.RegisterServiceAsync("IdentityManagerServiceType", serviceContext => new IdentityManagerService(serviceContext, communicationContext)).GetAwaiter().GetResult();

            communicationContext.WebHost.Run();
        }
        private static AspNetCoreCommunicationContext CreateAspNetCoreCommunicationContext()
        {
            var webHost = new WebHostBuilder().UseKestrel()
                                              .UseContentRoot(Directory.GetCurrentDirectory())
                                              .UseStartup<Startup>()
                                              .UseServiceFabricEndpoint("IdentityManagerServiceTypeEndpoint")
                                              .Build();

            return new AspNetCoreCommunicationContext(webHost);
        }
    }
}
