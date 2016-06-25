namespace Nature.Core.Data.Uow
{
    /// <summary>
    /// 工作单元接口
    /// </summary>
    public interface IUnitOfWork : IActiveUnitOfWork, IUnitOfWorkCompleteHandle
    {
        /// <summary>
        /// 根据设置开始一个工作单元
        /// </summary>
        /// <param name="options"></param>
        void Begin(UnitOfWorkOptions options);
    }
}