using System;
using System.Threading.Tasks;

namespace Nature.Core.Data.Uow
{
    /// <summary>
    /// 工作单元提交处理
    /// </summary>
    public interface IUnitOfWorkCompleteHandle : IDisposable
    {
        /// <summary>
        /// 提交本次工作单元
        /// </summary>
        void Complete();

        /// <summary>
        /// 异步提交本次工作单元
        /// </summary>
        /// <returns></returns>
        Task CompleteAsync();
    }
}