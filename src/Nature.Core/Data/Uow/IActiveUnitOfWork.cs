using System;
using System.Threading.Tasks;

namespace Nature.Core.Data.Uow
{
    public interface IActiveUnitOfWork
    {
        /// <summary>
        /// 完成时事件
        /// </summary>
        event EventHandler Completed;

        /// <summary>
        /// 失败时事件
        /// </summary>
        event EventHandler<UnitOfWorkFailedEventArgs> Failed;

        /// <summary>
        /// 释放时事件
        /// </summary>
        event EventHandler Disposed;

        /// <summary>
        /// 是否已释放过
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// 工作单元选项
        /// </summary>
        UnitOfWorkOptions Options { get; }

        /// <summary>
        /// 保存更改
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// 异步保存更改
        /// </summary>
        /// <returns></returns>
        Task SaveChangesAsync();
    }

    public class UnitOfWorkFailedEventArgs : EventArgs
    {
        public UnitOfWorkFailedEventArgs(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; private set; }
    }
}