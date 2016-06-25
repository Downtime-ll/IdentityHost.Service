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
        /// ��ǰ������Ԫ
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
                            //û�б�Ҫ�ظ�����
                            return;
                        }

                        UnitOfWorkDictionary.TryRemove(unitOfWorkKey, out unitOfWork);
                    }

                    CallContext.LogicalSetData(ContextKey, null);
                }

                if (value == null)
                {
                    //Ϊ�ղ�������
                    return;
                }

                unitOfWorkKey = Guid.NewGuid().ToString();
                //���������ܳ�����˲����쳣����
                UnitOfWorkDictionary.TryAdd(unitOfWorkKey, value);
                //���õ�ǰ�����������еĹ�����Ԫ
                CallContext.LogicalSetData(ContextKey, unitOfWorkKey);
            }
        }
    }
}