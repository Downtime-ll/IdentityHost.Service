using System;
using System.Transactions;

namespace Nature.Core.Data.Uow
{
    public class UnitOfWorkOptions
    {
        ///// <summary>
        ///// 工作单元是否是事物
        ///// </summary>
        //private bool IsTransactional { get; set; }

        /// <summary>
        /// 超时时间
        /// </summary>
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// 事务隔离级别
        /// </summary>
        public IsolationLevel? IsolationLevel { get; set; }
    }
}