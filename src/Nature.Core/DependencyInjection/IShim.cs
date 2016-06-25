using System;

namespace Nature.Core.DependencyInjection
{
    /// <summary>
    /// 容器垫片,需要访问容器时继承
    /// <code>
    ///
    /// </code>
    /// </summary>
    public interface IShim
    {
        IServiceProvider ServiceProvider { get; set; }
    }
}