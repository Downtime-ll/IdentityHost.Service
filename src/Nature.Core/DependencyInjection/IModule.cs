using Microsoft.Extensions.DependencyInjection;

namespace Nature.Core.DependencyInjection
{
    /// <summary>
    /// 容器注入接口
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="serviceCollection"></param>
        void Configure(IServiceCollection serviceCollection);

        /// <summary>
        /// 延迟配置
        /// </summary>
        /// <param name="serviceCollection"></param>
        void PostConfigure(IServiceCollection serviceCollection);
    }
}