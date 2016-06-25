using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Nature.Core.Data
{
    /// <summary>
    /// 仓储类基类
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public abstract class RepositoryBase<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        #region Implementation of IRepository<TEntity,TPrimaryKey>

        /// <summary>
        /// 获取所有数据 返回 IQueryable
        /// </summary>
        /// <returns></returns>
        public abstract IQueryable<TEntity> FindAll();

        public IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate)
        {
            return FindAll().Where(predicate);
        }

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        public virtual List<TEntity> FindAllList()
        {
            return FindAll().ToList();
        }

        /// <summary>
        /// 异步获取所有数据List
        /// </summary>
        /// <returns></returns>
        public virtual Task<List<TEntity>> FindAllListAsync()
        {
            return Task.FromResult(FindAllList());
        }

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual List<TEntity> FindAllList(Expression<Func<TEntity, bool>> predicate)
        {
            return FindAll().Where(predicate).ToList();
        }

        /// <summary>
        /// 异步获取所有数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual Task<List<TEntity>> FindAllListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(FindAllList(predicate));
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryMethod"></param>
        /// <returns></returns>
        public virtual T Query<T>(Func<IQueryable<TEntity>, T> queryMethod)
        {
            return queryMethod(FindAll());
        }

        /// <summary>
        /// 根据主键查询数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TEntity Find(TPrimaryKey id)
        {
            var entity = FirstOrDefault(id);
            if (entity == null)
            {
                throw new NatureException("查找不到数据. 实体类型: " + typeof(TEntity).FullName + ", 主键: " + id);
            }

            return entity;
        }

        /// <summary>
        /// 异步根据主键查询数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> FindAsync(TPrimaryKey id)
        {
            var entity = await FirstOrDefaultAsync(id);
            if (entity == null)
            {
                throw new NatureException("查找不到数据. 实体类型: " + typeof(TEntity).FullName + ", 主键: " + id);
            }

            return entity;
        }

        public virtual TEntity FirstOrDefault(TPrimaryKey id)
        {
            return FindAll().FirstOrDefault(CreateEqualityExpressionForId(id));
        }

        public virtual Task<TEntity> FirstOrDefaultAsync(TPrimaryKey id)
        {
            return Task.FromResult(FirstOrDefault(id));
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public abstract TEntity Insert(TEntity entity);

        /// <summary>
        /// 异步插入数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual Task<TEntity> InsertAsync(TEntity entity)
        {
            return Task.FromResult(Insert(entity));
        }

        /// <summary>
        /// 插入或者更新数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual TEntity InsertOrUpdate(TEntity entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 异步插入或者更新数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual Task<TEntity> InsertOrUpdateAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public abstract TEntity Update(TEntity entity);

        /// <summary>
        /// 异步更新数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual Task<TEntity> UpdateAsync(TEntity entity)
        {
            return Task.FromResult(Update(entity));
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <param name="entity"></param>
        public abstract void Delete(TEntity entity);

        /// <summary>
        /// 异步删除一条数据
        /// </summary>
        /// <param name="entity"></param>
        public virtual async Task DeleteAsync(TEntity entity)
        {
            Delete(entity);
        }

        /// <summary>
        /// 根据主键删除数据
        /// </summary>
        /// <param name="id"></param>
        public abstract void Delete(TPrimaryKey id);

        /// <summary>
        /// 异步根据主键删除数据
        /// </summary>
        /// <param name="id"></param>
        public async Task DeleteAsync(TPrimaryKey id)
        {
            Delete(id);
        }

        /// <summary>
        /// 根据表达式删除数据
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            foreach (var entity in FindAll(predicate).ToList())
            {
                Delete(entity);
            }
        }

        /// <summary>
        /// 异步根据表达式删除数据
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            Delete(predicate);
        }

        /// <summary>
        /// 获取所有数据数量
        /// </summary>
        /// <returns></returns>
        public virtual int Count()
        {
            return FindAll().Count();
        }

        /// <summary>
        /// 异步获取所有数据数据
        /// </summary>
        /// <returns></returns>
        public virtual Task<int> CountAsync()
        {
            return Task.FromResult(Count());
        }

        /// <summary>
        /// 根据表达式查询数量
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return FindAll().Where(predicate).Count();
        }

        /// <summary>
        /// 异步根据表达式查询数量
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(Count(predicate));
        }

        #endregion Implementation of IRepository<TEntity,TPrimaryKey>

        /// <summary>
        /// 构建id查询表达式
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected static Expression<Func<TEntity, bool>> CreateEqualityExpressionForId(TPrimaryKey id)
        {
            var lambdaParam = Expression.Parameter(typeof(TEntity));

            var lambdaBody = Expression.Equal(
                Expression.PropertyOrField(lambdaParam, "Id"),
                Expression.Constant(id, typeof(TPrimaryKey))
                );

            return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
        }
    }
}