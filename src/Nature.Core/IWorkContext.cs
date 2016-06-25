using System;

namespace Nature.Core
{
    public interface IWorkContext
    {
        /// <summary>
        /// Ioc 容器服务
        /// </summary>
        IServiceProvider ServiceProvider { get; }

        void SetState<T>(string name, T value);

        T GetState<T>(string name);
    }
}