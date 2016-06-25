using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Nature.Core.DependencyInjection;

namespace Nature.Core.Data
{
    public interface IRepository : ITransientDependency
    {
    }

    public interface IRepository<TEntity> :IRepository
        where TEntity : IEntity
    {
        #region Select/Find/Query

        /// <summary>
        /// 获取所有数据 返回 IQueryable
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> FindAll();

        IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        List<TEntity> FindAllList();

        /// <summary>
        /// 异步获取所有数据List
        /// </summary>
        /// <returns></returns>
        Task<List<TEntity>> FindAllListAsync();

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        List<TEntity> FindAllList(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 异步获取所有数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<List<TEntity>> FindAllListAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryMethod"></param>
        /// <returns></returns>
        T Query<T>(Func<IQueryable<TEntity>, T> queryMethod);

       

        #endregion Select/Find/Query

        #region Insert

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TEntity Insert(TEntity entity);

        /// <summary>
        /// 异步插入数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<TEntity> InsertAsync(TEntity entity);

        /// <summary>
        /// 插入或者更新数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TEntity InsertOrUpdate(TEntity entity);

        /// <summary>
        /// 异步插入或者更新数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<TEntity> InsertOrUpdateAsync(TEntity entity);

        #endregion Insert

        #region Update

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TEntity Update(TEntity entity);

        /// <summary>
        /// 异步更新数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<TEntity> UpdateAsync(TEntity entity);

        #endregion Update

        #region Delete

        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <param name="entity"></param>
        void Delete(TEntity entity);

        /// <summary>
        /// 异步删除一条数据
        /// </summary>
        /// <param name="entity"></param>
        Task DeleteAsync(TEntity entity);

       

        /// <summary>
        /// 根据表达式删除数据
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        void Delete(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 异步根据表达式删除数据
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion Delete

        #region Aggregates

        /// <summary>
        /// 获取所有数据数量
        /// </summary>
        /// <returns></returns>
        int Count();

        /// <summary>
        /// 异步获取所有数据数据
        /// </summary>
        /// <returns></returns>
        Task<int> CountAsync();

        /// <summary>
        /// 根据表达式查询数量
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int Count(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 异步根据表达式查询数量
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion Aggregates
    }
}