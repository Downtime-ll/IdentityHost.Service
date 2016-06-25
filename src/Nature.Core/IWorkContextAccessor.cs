using System;
using Microsoft.AspNetCore.Http;
using Nature.Core.DependencyInjection;

namespace Nature.Core
{
    public interface IWorkContextAccessor
    {
        IWorkContext GetContext();

        IWorkContext GetContext(HttpContext httpContext);

        IWorkContextScope CreateWorkContextScope(HttpContext httpContext);

        IWorkContextScope CreateWorkContextScope();
    }

    public interface IWorkContextStateProvider : IDependency
    {
        Func<IWorkContext, T> Get<T>(string name);
    }

    public interface IWorkContextScope : IDisposable
    {
        IWorkContext WorkContext { get; }

        TService Resolve<TService>();
    }
}