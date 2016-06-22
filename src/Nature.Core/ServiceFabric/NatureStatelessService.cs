using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Nature.Core.ServiceFabric
{
    public class NatureStatelessService : StatelessService
    {
        protected virtual IServiceProvider RequestProvider { get; set; }

        public NatureStatelessService(StatelessServiceContext serviceContext) : base(serviceContext)
        {
        }

        protected IServiceScope BegionScope()
        {
            if (AppContext.CurrentHttpContext == null)
            {
                var scope = AppContext.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
                RequestProvider = scope.ServiceProvider;
                return new CallbackServiceScope(() => RequestProvider = null, scope);
            }

            RequestProvider = AppContext.CurrentHttpContext.RequestServices;
            return new HttpService(AppContext.ApplicationServices);
        }

        public class CallbackServiceScope : IServiceScope, IDisposable
        {
            private readonly Action _disposeCallback;
            private readonly IServiceScope _serviceScope;

            public CallbackServiceScope(Action disposeCallback,IServiceScope serviceScope)
            {
                _disposeCallback = disposeCallback;
                _serviceScope = serviceScope;
            }

            public void Dispose()
            {
                _disposeCallback();
                _serviceScope.Dispose();
            }

            public IServiceProvider ServiceProvider => _serviceScope.ServiceProvider;
        }

        public class HttpService : IServiceScope
        {
            public HttpService(IServiceProvider serviceProvider)
            {
                ServiceProvider = serviceProvider;
            }

            public void Dispose()
            {
            }

            public IServiceProvider ServiceProvider { get; }
        }
    }
}
