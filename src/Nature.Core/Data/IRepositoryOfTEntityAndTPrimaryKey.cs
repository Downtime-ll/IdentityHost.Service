using System.Threading.Tasks;

namespace Nature.Core.Data
{
    /// <summary>
    /// 仓储类接口
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public interface IRepository<TEntity, in TPrimaryKey> : IRepository<TEntity>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        /// <summary>
        /// 根据主键查询数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity Find(TPrimaryKey id);

        /// <summary>
        /// 异步根据主键查询数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TEntity> FindAsync(TPrimaryKey id);

        /// <summary>
        /// 根据主键删除数据
        /// </summary>
        /// <param name="id"></param>
        void Delete(TPrimaryKey id);

        /// <summary>
        /// 异步根据主键删除数据
        /// </summary>
        /// <param name="id"></param>
        Task DeleteAsync(TPrimaryKey id);
    }
}