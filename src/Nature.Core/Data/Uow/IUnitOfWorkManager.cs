namespace Nature.Core.Data.Uow
{
    /// <summary>
    /// 工作单元管理
    /// </summary>
    public interface IUnitOfWorkManager
    {
        /// <summary>
        /// 当前工作单元
        /// </summary>
        IActiveUnitOfWork Current { get; }

        /// <summary>
        /// 启动工作单元
        /// </summary>
        /// <returns></returns>
        IUnitOfWorkCompleteHandle Begin();

        /// <summary>
        /// 启动工作单元
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        IUnitOfWorkCompleteHandle Begin(UnitOfWorkOptions options);
    }
}