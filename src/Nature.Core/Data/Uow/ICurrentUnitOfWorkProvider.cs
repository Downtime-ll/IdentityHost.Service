using Nature.Core.DependencyInjection;

namespace Nature.Core.Data.Uow
{
    public interface ICurrentUnitOfWorkProvider : ITransientDependency
    {
        IUnitOfWork Current { get; set; }
    }
}