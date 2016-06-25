using System.Threading.Tasks;

namespace Nature.Core.Data.Uow
{
    internal class InnerUnitOfWorkCompleteHandle : IUnitOfWorkCompleteHandle
    {
        private volatile bool _isCompleteCalled;
        private volatile bool _isDisposed;

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
        }

        /// <summary>
        /// 提交本次工作单元
        /// </summary>
        public void Complete()
        {
            _isCompleteCalled = true;
        }

        /// <summary>
        /// 异步提交本次工作单元
        /// </summary>
        /// <returns></returns>
        public async Task CompleteAsync()
        {
            _isCompleteCalled = true;
        }
    }
}