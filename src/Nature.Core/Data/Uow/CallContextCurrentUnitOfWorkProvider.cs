using System;
using System.Collections.Concurrent;
using System.Runtime.Remoting.Messaging;

namespace Nature.Core.Data.Uow
{
    public class CallContextCurrentUnitOfWorkProvider : ICurrentUnitOfWorkProvider
    {
        private const string ContextKey = "Nature.UnitOfWork.Current";

        private static readonly ConcurrentDictionary<string, IUnitOfWork> UnitOfWorkDictionary
            = new ConcurrentDictionary<string, IUnitOfWork>();

        private IUnitOfWorkManager unitOfWorkManager;

        /// <summary>
        /// 当前工作单元
        /// </summary>
        public IUnitOfWork Current
        {
            get
            {
                var unitOfWorkKey = CallContext.LogicalGetData(ContextKey) as string;
                if (unitOfWorkKey == null)
                {
                    return null;
                }
                IUnitOfWork unitOfWork;
                if (!UnitOfWorkDictionary.TryGetValue(unitOfWorkKey, out unitOfWork))
                {
                    CallContext.LogicalSetData(ContextKey, null);
                    return null;
                }
                if (unitOfWork.IsDisposed)
                {
                    CallContext.LogicalSetData(ContextKey, null);
                    UnitOfWorkDictionary.TryRemove(unitOfWorkKey, out unitOfWork);
                    return null;
                }

                return unitOfWork;
            }
            set
            {
                var unitOfWorkKey = CallContext.LogicalGetData(ContextKey) as string;
                if (unitOfWorkKey != null)
                {
                    IUnitOfWork unitOfWork;
                    if (UnitOfWorkDictionary.TryGetValue(unitOfWorkKey, out unitOfWork))
                    {
                        if (unitOfWork == value)
                        {
                            //没有必要重复插入
                            return;
                        }

                        UnitOfWorkDictionary.TryRemove(unitOfWorkKey, out unitOfWork);
                    }

                    CallContext.LogicalSetData(ContextKey, null);
                }

                if (value == null)
                {
                    //为空不做处理
                    return;
                }

                unitOfWorkKey = Guid.NewGuid().ToString();
                //几乎不可能出错，因此不做异常处理
                UnitOfWorkDictionary.TryAdd(unitOfWorkKey, value);
                //设置当前调用上下文中的工作单元
                CallContext.LogicalSetData(ContextKey, unitOfWorkKey);
            }
        }
    }
}