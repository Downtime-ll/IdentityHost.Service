namespace Nature.Core.DependencyInjection
{
    /// <summary>
    /// 生命周期 "单例" 基础接口,所有继承此类在容器中创建都会以单例形式存在
    /// </summary>
    public interface ISingletonDependency : IDependency
    {
    }
}
