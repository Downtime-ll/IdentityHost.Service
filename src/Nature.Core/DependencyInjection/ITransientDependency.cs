namespace Nature.Core.DependencyInjection
{
    /// <summary>
    /// 所有继承此类在容器中创建都会以是新的实例
    /// </summary>
    public interface ITransientDependency : IDependency
    {
    }
}