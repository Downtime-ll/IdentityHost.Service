using System.Threading.Tasks;

namespace Nature.Core.Data.Uow
{
    public class NullUnitOfWork : UnitOfWorkBase
    {
        #region Overrides of UnitOfWorkBase

        /// <summary>
        /// 保存更改
        /// </summary>
        public override void SaveChanges()
        {
        }

        /// <summary>
        /// 异步保存更改
        /// </summary>
        /// <returns></returns>
        public override async Task SaveChangesAsync()
        {
        }

        /// <summary>
        /// 开始工作单元
        /// </summary>
        protected override void BeginUow()
        {
        }

        /// <summary>
        /// 完成工作单元
        /// </summary>
        protected override void CompleteUow()
        {
        }

        /// <summary>
        /// 异步执行完成工作单元
        /// </summary>
        /// <returns></returns>
        protected async override Task CompleteUowAsync()
        {
        }

        /// <summary>
        /// 释放当前工作单元
        /// </summary>
        protected override void DisposeUow()
        {
        }

        #endregion
    }
}
