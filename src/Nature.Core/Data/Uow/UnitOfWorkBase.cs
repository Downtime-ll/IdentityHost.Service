using System;
using System.Threading.Tasks;
using Nature.Core.Extensions;

namespace Nature.Core.Data.Uow
{
    /// <summary>
    /// 工作单元抽象基类
    /// </summary>
    public abstract class UnitOfWorkBase : IUnitOfWork
    {
        /// <summary>
        /// 是否执行过begin
        /// </summary>
        private bool _isBeginCalledBefore;

        /// <summary>
        /// 是否执行过Complete
        /// </summary>
        private bool _isCompleteCalledBefore;

        /// <summary>
        /// 成功
        /// </summary>
        private bool _succeed;

        /// <summary>
        /// 异常
        /// </summary>
        private Exception _exception;

        #region Implementation of IActiveUnitOfWork

        public event EventHandler Completed;

        public event EventHandler<UnitOfWorkFailedEventArgs> Failed;

        public event EventHandler Disposed;

        /// <summary>
        /// 是否已释放过
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// 工作单元选项
        /// </summary>
        public UnitOfWorkOptions Options { get; private set; }

        /// <summary>
        /// 保存更改
        /// </summary>
        public abstract void SaveChanges();

        /// <summary>
        /// 异步保存更改
        /// </summary>
        /// <returns></returns>
        public abstract Task SaveChangesAsync();

        #endregion Implementation of IActiveUnitOfWork

        #region Implementation of IDisposable

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            if (!_succeed)
            {
                OnFailed(_exception);
            }

            DisposeUow();
            OnDisposed();
        }

        #endregion Implementation of IDisposable

        #region Implementation of IUnitOfWorkCompleteHandle

        /// <summary>
        /// 提交本次工作单元
        /// </summary>
        public void Complete()
        {
            CheckComplete();

            try
            {
                CompleteUow();
                _succeed = true;
                OnCompleted();
            }
            catch (Exception ex)
            {
                _exception = ex;
                throw;
            }
        }

        /// <summary>
        /// 异步提交本次工作单元
        /// </summary>
        /// <returns></returns>
        public async Task CompleteAsync()
        {
            CheckComplete();

            try
            {
                await CompleteUowAsync();
                _succeed = true;
                OnCompleted();
            }
            catch (Exception ex)
            {
                _exception = ex;
                throw;
            }
        }

        /// <summary>
        /// 根据设置开始一个工作单元
        /// </summary>
        /// <param name="options"></param>
        public void Begin(UnitOfWorkOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            //检验是否执行过
            CheckBegin();
            Options = options;

            //执行工作单元
            BeginUow();
        }

        #endregion Implementation of IUnitOfWorkCompleteHandle

        /// <summary>
        /// 开始工作单元
        /// </summary>
        protected abstract void BeginUow();

        /// <summary>
        /// 完成工作单元
        /// </summary>
        protected abstract void CompleteUow();

        /// <summary>
        /// 异步执行完成工作单元
        /// </summary>
        /// <returns></returns>
        protected abstract Task CompleteUowAsync();

        /// <summary>
        /// 释放当前工作单元
        /// </summary>
        protected abstract void DisposeUow();

        protected virtual void OnCompleted()
        {
            Completed.InvokeSafely(this);
        }

        protected virtual void OnFailed(Exception exception)
        {
            Failed.InvokeSafely(this, new UnitOfWorkFailedEventArgs(exception));
        }

        protected virtual void OnDisposed()
        {
            Disposed.InvokeSafely(this);
        }

        private void CheckBegin()
        {
            if (_isBeginCalledBefore)
            {
                throw new NatureException("当前工作单元已被执行过，不能重复执行。");
            }

            _isBeginCalledBefore = true;
        }

        private void CheckComplete()
        {
            if (_isCompleteCalledBefore)
            {
                throw new NatureException("当前工作单元已完成!");
            }

            _isCompleteCalledBefore = true;
        }
    }
}