using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Data.Abstractions
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity Add(TEntity entity, bool autoSave = true);
        IEnumerable<TEntity> AddAll(IEnumerable<TEntity> entities, bool autoSave = true);
        Task<IEnumerable<TEntity>> AddAllAsync(IEnumerable<TEntity> entities, bool autoSave = true);
        Task<TEntity> AddAsync(TEntity entity, bool autoSave = true);
        //Task<IEnumerable<TEntity>> AddOrUpdateAllAsync(TEntity[] entities, bool autoSave = true);
        //Task<IEnumerable<TEntity>> AddOrUpdateAllAsync(Expression<Func<TEntity, object>> identifierExpression, TEntity[] entities, bool autoSave = true);
        Task<TEntity> AddOrUpdateAsync(TEntity entity, bool autoSave = true);
        Task<TEntity> AddOrUpdateAsync(Expression<Func<TEntity, object>> identifierExpression, TEntity entity, bool autoSave = true);
        IQueryable<TEntity> DeferredSelectAll();
        IQueryable<TEntity> DeferredSelectAll(string orderByProperties);
        IQueryable<TEntity> DeferredSelectAll(Expression<Func<TEntity, bool>> filterExpression);
        IQueryable<TEntity> DeferredSelectAll(string orderByProperties, Expression<Func<TEntity, bool>> filterExpression);
        IQueryable<TEntity> DeferredSelectAll(int page, int pageSize);
        IQueryable<TEntity> DeferredSelectAll(string orderByProperties, int page, int pageSize);
        IQueryable<TEntity> DeferredSelectAll(int page, int pageSize, Expression<Func<TEntity, bool>> filterExpression);
        IQueryable<TEntity> DeferredSelectAll(string orderByProperties, int page, int pageSize, Expression<Func<TEntity, bool>> filterExpression);
        IQueryable<TEntity> DeferredWhere(Expression<Func<TEntity, bool>> condition);
        IQueryable<TEntity> DeferredWhere(Expression<Func<TEntity, bool>> condition, string orderByProperties);
        IQueryable<TEntity> DeferredWhere(Expression<Func<TEntity, bool>> condition, int page, int pageSize);
        IQueryable<TEntity> DeferredWhere(Expression<Func<TEntity, bool>> condition, string orderByProperties, int page, int pageSize);
        Task<IList<TEntity>> DynamicWhere(string orderByProperties, int page, int pageSize, string predicate, params object[] args);
        Task<IList<TEntity>> DynamicWhere(string orderByProperties, string predicate, params object[] args);
        bool Delete(TEntity entity, bool autoSave = true);
        bool DeleteAll(IEnumerable<TEntity> entities, bool autoSave = true);
        Task<bool> DeleteAllAsync(IEnumerable<TEntity> entities, bool autoSave = true);
        Task<bool> DeleteAsync(TEntity entity, bool autoSave = true);
        void Dispose();
        TEntity Find(params object[] keyValue);
        Task<TEntity> FindAsync(params object[] keyValue);
        bool SaveChanges();
        Task<bool> SaveChangesAsync();
        IList<TEntity> SelectAll();
        IList<TEntity> SelectAll(string orderByProperties);
        IList<TEntity> SelectAll(Expression<Func<TEntity, bool>> filterExpression);
        IList<TEntity> SelectAll(string orderByProperties, Expression<Func<TEntity, bool>> filterExpression);
        IList<TEntity> SelectAll(int page, int pageSize);
        IList<TEntity> SelectAll(string orderByProperties, int page, int pageSize);
        IList<TEntity> SelectAll(int page, int pageSize, Expression<Func<TEntity, bool>> filterExpression);
        IList<TEntity> SelectAll(string orderByProperties, int page, int pageSize, Expression<Func<TEntity, bool>> filterExpression);
        Task<IList<TEntity>> SelectAllAsync();
        Task<IList<TEntity>> SelectAllAsync(string orderByProperties);
        Task<IList<TEntity>> SelectAllAsync(Expression<Func<TEntity, bool>> filterExpression);
        Task<IList<TEntity>> SelectAllAsync(string orderByProperties, Expression<Func<TEntity, bool>> filterExpression);
        Task<IList<TEntity>> SelectAllAsync(int page, int pageSize);
        Task<IList<TEntity>> SelectAllAsync(string orderByProperties, int page, int pageSize);
        Task<IList<TEntity>> SelectAllAsync(int page, int pageSize, Expression<Func<TEntity, bool>> filterExpression);
        Task<IList<TEntity>> SelectAllAsync(string orderByProperties, int page, int pageSize, Expression<Func<TEntity, bool>> filterExpression);
        //TEntity Update(TEntity entity, bool autoSave = true);
        //TEntity Update(TEntity entity, bool autoSave = true, params string[] fields);
        Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = true);
        //Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = true, params string[] fields);
        IList<TEntity> Where(Expression<Func<TEntity, bool>> condition);
        IList<TEntity> Where(Expression<Func<TEntity, bool>> condition, string orderByProperties);
        IList<TEntity> Where(Expression<Func<TEntity, bool>> condition, int page, int pageSize);
        IList<TEntity> Where(Expression<Func<TEntity, bool>> condition, string orderByProperties, int page, int pageSize);
        Task<IList<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> condition);
        Task<IList<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> condition, string orderByProperties);
        Task<IList<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> condition, int page, int pageSize);
        Task<IList<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> condition, string orderByProperties, int page, int pageSize);
    }

    public interface IRepository<TEntity, TIEntity> : IRepository<TEntity>
        where TEntity : class, TIEntity
    {
    }
}