namespace Nature.Core.Data.Uow
{
    public class UnitOfWorkManager : IUnitOfWorkManager
    {
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;
        private readonly IWorkContextAccessor _workContextAccessor;

        public UnitOfWorkManager(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider,
                                 IWorkContextAccessor workContextAccessor)
        {
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
            _workContextAccessor = workContextAccessor;
        }

        #region Implementation of IUnitOfWorkManager

        /// <summary>
        /// 当前工作单元
        /// </summary>
        public IActiveUnitOfWork Current { get { return _currentUnitOfWorkProvider.Current; } }

        /// <summary>
        /// 启动工作单元
        /// </summary>
        /// <returns></returns>
        public IUnitOfWorkCompleteHandle Begin()
        {
            return Begin(new UnitOfWorkOptions());
        }

        /// <summary>
        /// 启动工作单元
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public IUnitOfWorkCompleteHandle Begin(UnitOfWorkOptions options)
        {
            if (_currentUnitOfWorkProvider.Current != null)
            {
                return new InnerUnitOfWorkCompleteHandle();
            }

            var uow = (IUnitOfWork) _workContextAccessor.GetContext().ServiceProvider.GetService(typeof(IUnitOfWork));

            uow.Completed += (sender, args) =>
            {
                _currentUnitOfWorkProvider.Current = null;
            };

            uow.Failed += (sender, args) =>
            {
                _currentUnitOfWorkProvider.Current = null;
            };

            uow.Begin(options);
            _currentUnitOfWorkProvider.Current = uow;

            return uow;
        }

        #endregion Implementation of IUnitOfWorkManager
    }
}