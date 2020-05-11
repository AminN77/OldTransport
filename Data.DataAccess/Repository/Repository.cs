using Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Cross.Abstractions;
using System.Linq.Dynamic.Core;

namespace Data.DataAccess.Repository
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        private bool _disposed;
        private const bool AutoSave = true;
        private IMyContext _dbContext;
        private DbSet<TEntity> _dbSet;
        private IUtility _utility;

        public Repository(IMyContext context, IUtility utility)
        {
            _dbContext = context;
            _dbSet = _dbContext.Set<TEntity>();
            _utility = utility;
        }

        public TEntity Add(TEntity entity, bool autoSave = AutoSave)
        {
            _dbSet.Add(entity);
            return SaveChanges(entity, autoSave);
        }

        public async Task<TEntity> AddAsync(TEntity entity, bool autoSave = AutoSave)
        {
            await _dbSet.AddAsync(entity);
            return await SaveChangesAsync(entity, autoSave);
        }

        public IEnumerable<TEntity> AddAll(IEnumerable<TEntity> entities, bool autoSave = AutoSave)
        {
            _dbSet.AddRange(entities);
            return SaveChanges(entities, autoSave);
        }

        public async Task<IEnumerable<TEntity>> AddAllAsync(IEnumerable<TEntity> entities, bool autoSave = AutoSave)
        {
            await _dbSet.AddRangeAsync(entities);
            return await SaveChangesAsync(entities, autoSave);
        }

        //public async Task<IEnumerable<TEntity>> AddOrUpdateAllAsync(TEntity[] entities, bool autoSave = AutoSave)
        //{
        //    _dbSet.AddOrUpdate(entities);
        //    return await SaveChangesAsync(entities, autoSave);
        //}

        public async Task<TEntity> AddOrUpdateAsync(TEntity entity, bool autoSave = AutoSave)
        {
            _dbSet.AddOrUpdate(entity);
            return await SaveChangesAsync(entity, autoSave);
        }

//        public async Task<IEnumerable<TEntity>> AddOrUpdateAllAsync(Expression<Func<TEntity, object>> identifierExpression, TEntity[] entities, bool autoSave = AutoSave)
//        {
//            _dbSet.AddOrUpdate(identifierExpression, entities);
//            return await SaveChangesAsync(entities, autoSave);
//        }

        public async Task<TEntity> AddOrUpdateAsync(Expression<Func<TEntity, object>> identifierExpression, TEntity entity, bool autoSave = AutoSave)
        {
            _dbSet.AddOrUpdate(identifierExpression, entity);
            return await SaveChangesAsync(entity, autoSave);
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = AutoSave)
        {
            //var d = _dbSet..GetType();
            //var dd = d.GetProperties();
            //var ddd = dd.Where(prop => prop.IsDefined(typeof(KeyAttribute), true));
            //var dddd = ddd.Select(prop => prop.GetValue(entity)).ToArray();

            //var keyValue = _dbSet.GetType().GetProperties().Where(prop => prop.IsDefined(typeof(KeyAttribute), true)).Select(prop => prop.GetValue(toBeUpdatedEntity)).ToArray();
            //TEntity attachedEntity;
            //attachedEntity = await _dbSet.FindAsync(keyValue);

            //_dbContext.Entry(attachedEntity).CurrentValues.SetValues(toBeUpdatedEntity);
            
            //bool isDetached = _dbContext.Entry(entity).State == EntityState.Detached;
            //if (isDetached)
            //    _dbSet.Attach(entity);
            _dbSet.Update(entity);
            return await SaveChangesAsync(entity, autoSave);
        }

        //public TEntity Update(TEntity entity, bool autoSave = AutoSave, params string[] fields)
        //{
        //    var updateTask = Update(entity, true, autoSave, fields);
        //    updateTask.Wait();
        //    return updateTask.Result;
        //}

        //public async Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = AutoSave)
        //{
        //    return await Update(entity, true, autoSave, _utility.GetEntityProperties(entity));
        //}

        //private async Task<TEntity> Update(TEntity entity, bool isAsync, bool autoSave, params string[] fields)
        //{
        //    var entry = _dbContext.Entry(entity);
        //    if (entry == null) return null;
        //    var state = entry.State;
        //    if (state != EntityState.Detached)
        //    {
        //        entity = _dbSet.Attach(entity);
        //        foreach (var field in fields)
        //        {
        //            var property = entry.Property(field);
        //            if (property == null) continue;
        //            property.IsModified = true;
        //        }
        //    }
        //    else
        //    {
        //        var keyValue = _dbSet.Create().GetType().GetProperties().Where(prop => prop.IsDefined(typeof(KeyAttribute), true)).Select(prop => prop.GetValue(entity)).ToArray();
        //        TEntity attachedEntity;
        //        if (isAsync)
        //        {
        //            attachedEntity = await _dbSet.FindAsync(keyValue);
        //        }
        //        else
        //        {
        //            attachedEntity = _dbSet.Find(keyValue);
        //        }
        //        if (attachedEntity != null)
        //        {
        //            foreach (var field in fields)
        //            {
        //                // var attachedEntry = _dbContext.Entry(attachedEntity);
        //                var toBeSetProperty = attachedEntity.GetType().GetProperty(field);
        //                if (toBeSetProperty == null || !toBeSetProperty.CanWrite) continue;
        //                var toBeGetProperty = entity.GetType().GetProperty(field);
        //                if (toBeGetProperty == null || !toBeGetProperty.CanRead) continue;
        //                toBeSetProperty.SetValue(attachedEntity, toBeGetProperty.GetValue(entity));
        //            }
        //        }
        //        else
        //        {
        //            entry.State = EntityState.Modified; // This should attach entity
        //        }
        //    }
        //    return isAsync ? await SaveChangesAsync(entity, autoSave) : SaveChanges(entity, autoSave);
        //}

        //public async Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = AutoSave, params string[] fields)
        //{
        //    return await Update(entity, true, autoSave, fields);
        //}

        public bool Delete(TEntity entity, bool autoSave = AutoSave)
        {
            _dbSet.Remove(entity);
            return SaveChanges(autoSave);
        }

        public async Task<bool> DeleteAsync(TEntity entity, bool autoSave = AutoSave)
        {
            _dbSet.Remove(entity);
            return await SaveChangesAsync(autoSave);
        }

        public bool DeleteAll(IEnumerable<TEntity> entities, bool autoSave = AutoSave)
        {
            _dbSet.RemoveRange(entities);
            return SaveChanges(autoSave);
        }

        public async Task<bool> DeleteAllAsync(IEnumerable<TEntity> entities, bool autoSave = AutoSave)
        {
            _dbSet.RemoveRange(entities);
            return await SaveChangesAsync(autoSave);
        }

        public TEntity Find(params object[] keyValue)
        {
            return _dbSet.Find(keyValue);
        }

        public async Task<TEntity> FindAsync(params object[] keyValue)
        {
            return await _dbSet.FindAsync(keyValue);
        }

        public IQueryable<TEntity> DeferredSelectAll()
        {
            return _dbSet.AsQueryable();
        }

        public IQueryable<TEntity> DeferredSelectAll(int page, int pageSize)
        {
            return DeferredSelectAll().DeferredPaginate(page, pageSize);
        }

        public IQueryable<TEntity> DeferredSelectAll(string orderByProperties)
        {
            return DeferredSelectAll().ApplyAllOrderBy(orderByProperties);
        }

        public IQueryable<TEntity> DeferredSelectAll(string orderByProperties, int page, int pageSize)
        {
            return DeferredSelectAll().ApplyAllOrderBy(orderByProperties).DeferredPaginate(page, pageSize);
        }

        public IQueryable<TEntity> DeferredSelectAll(Expression<Func<TEntity, bool>> filterExpression)
        {
            return _dbSet.AsQueryable().ApplyFilter(filterExpression);
        }

        public IQueryable<TEntity> DeferredSelectAll(int page, int pageSize, Expression<Func<TEntity, bool>> filterExpression)
        {
            return DeferredSelectAll().ApplyFilter(filterExpression).DeferredPaginate(page, pageSize);
        }

        public IQueryable<TEntity> DeferredSelectAll(string orderByProperties, Expression<Func<TEntity, bool>> filterExpression)
        {
            return DeferredSelectAll().ApplyFilter(filterExpression).ApplyAllOrderBy(orderByProperties);
        }

        public IQueryable<TEntity> DeferredSelectAll(string orderByProperties, int page, int pageSize, Expression<Func<TEntity, bool>> filterExpression)
        {
            return DeferredSelectAll().ApplyFilter(filterExpression).ApplyAllOrderBy(orderByProperties).DeferredPaginate(page, pageSize);
        }

        public IList<TEntity> SelectAll()
        {
            return DeferredSelectAll().ToList();
        }

        public IList<TEntity> SelectAll(int page, int pageSize)
        {
            return DeferredSelectAll().Paginate(page, pageSize);
        }

        public IList<TEntity> SelectAll(string orderByProperties)
        {
            return DeferredSelectAll().ApplyAllOrderBy(orderByProperties).ToList();
        }

        public IList<TEntity> SelectAll(string orderByProperties, int page, int pageSize)
        {
            return DeferredSelectAll().ApplyAllOrderBy(orderByProperties).Paginate(page, pageSize);
        }

        public async Task<IList<TEntity>> SelectAllAsync()
        {
            return await DeferredSelectAll().ToListAsync();
        }

        public async Task<IList<TEntity>> SelectAllAsync(int page, int pageSize)
        {
            return await DeferredSelectAll().PaginateAsync(page, pageSize);
        }

        public async Task<IList<TEntity>> SelectAllAsync(string orderByProperties)
        {
            return await DeferredSelectAll().ApplyAllOrderBy(orderByProperties).ToListAsync();
        }

        public async Task<IList<TEntity>> SelectAllAsync(string orderByProperties, int page, int pageSize)
        {
            return await DeferredSelectAll().ApplyAllOrderBy(orderByProperties).PaginateAsync(page, pageSize);
        }

        public IList<TEntity> SelectAll(Expression<Func<TEntity, bool>> filterExpression)
        {
            return DeferredSelectAll().ApplyFilter(filterExpression).ToList();
        }

        public IList<TEntity> SelectAll(int page, int pageSize, Expression<Func<TEntity, bool>> filterExpression)
        {
            return DeferredSelectAll().ApplyFilter(filterExpression).Paginate(page, pageSize);
        }

        public IList<TEntity> SelectAll(string orderByProperties, Expression<Func<TEntity, bool>> filterExpression)
        {
            return DeferredSelectAll().ApplyFilter(filterExpression).ApplyAllOrderBy(orderByProperties).ToList();
        }

        public IList<TEntity> SelectAll(string orderByProperties, int page, int pageSize, Expression<Func<TEntity, bool>> filterExpression)
        {
            return DeferredSelectAll().ApplyFilter(filterExpression).ApplyAllOrderBy(orderByProperties).Paginate(page, pageSize);
        }

        public async Task<IList<TEntity>> SelectAllAsync(Expression<Func<TEntity, bool>> filterExpression)
        {
            return await DeferredSelectAll().ApplyFilter(filterExpression).ToListAsync();
        }

        public async Task<IList<TEntity>> SelectAllAsync(int page, int pageSize, Expression<Func<TEntity, bool>> filterExpression)
        {
            return await DeferredSelectAll().ApplyFilter(filterExpression).PaginateAsync(page, pageSize);
        }

        public async Task<IList<TEntity>> SelectAllAsync(string orderByProperties, Expression<Func<TEntity, bool>> filterExpression)
        {
            return await DeferredSelectAll().ApplyFilter(filterExpression).ApplyAllOrderBy(orderByProperties).ToListAsync();
        }

        public async Task<IList<TEntity>> SelectAllAsync(string orderByProperties, int page, int pageSize, Expression<Func<TEntity, bool>> filterExpression)
        {
            return await DeferredSelectAll().ApplyFilter(filterExpression).ApplyAllOrderBy(orderByProperties).PaginateAsync(page, pageSize);
        }

        public IQueryable<TEntity> DeferredWhere(Expression<Func<TEntity, bool>> condition)
        {
            return _dbSet.Where(condition);
        }

        public IQueryable<TEntity> DeferredWhere(Expression<Func<TEntity, bool>> condition, int page, int pageSize)
        {
            return DeferredWhere(condition).DeferredPaginate(page, pageSize);
        }

        public IQueryable<TEntity> DeferredWhere(Expression<Func<TEntity, bool>> condition, string orderByProperties)
        {
            return DeferredWhere(condition).ApplyAllOrderBy(orderByProperties);
        }

        public IQueryable<TEntity> DeferredWhere(Expression<Func<TEntity, bool>> condition, string orderByProperties, int page, int pageSize)
        {
            return DeferredWhere(condition).ApplyAllOrderBy(orderByProperties).DeferredPaginate(page, pageSize);
        }

        public IList<TEntity> Where(Expression<Func<TEntity, bool>> condition)
        {
            return DeferredWhere(condition).ToList();
        }

        public IList<TEntity> Where(Expression<Func<TEntity, bool>> condition, int page, int pageSize)
        {
            return DeferredWhere(condition).Paginate(page, pageSize);
        }

        public IList<TEntity> Where(Expression<Func<TEntity, bool>> condition, string orderByProperties)
        {
            return DeferredWhere(condition).ApplyAllOrderBy(orderByProperties).ToList();
        }

        public IList<TEntity> Where(Expression<Func<TEntity, bool>> condition, string orderByProperties, int page, int pageSize)
        {
            return DeferredWhere(condition).ApplyAllOrderBy(orderByProperties).Paginate(page, pageSize);
        }

        public async Task<IList<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> condition)
        {
            return await DeferredWhere(condition).ToListAsync();
        }

        public async Task<IList<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> condition, int page, int pageSize)
        {
            return await DeferredWhere(condition).PaginateAsync(page, pageSize);
        }

        public async Task<IList<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> condition, string orderByProperties)
        {
            return await DeferredWhere(condition).ApplyAllOrderBy(orderByProperties).ToListAsync();
        }

        public async Task<IList<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> condition, string orderByProperties, int page, int pageSize)
        {
            return await DeferredWhere(condition).ApplyAllOrderBy(orderByProperties).PaginateAsync(page, pageSize);
        }

        public async Task<IList<TEntity>> DynamicWhere(string orderByProperties, int page, int pageSize, string predicate, params object[] args)
        {
            return await _dbSet.Where(predicate, args).OrderBy(orderByProperties).PaginateAsync(page, pageSize);
        }

        public async Task<IList<TEntity>> DynamicWhere(string orderByProperties, string predicate, params object[] args)
        {
            return await _dbSet.Where(predicate, args).OrderBy(orderByProperties).ToListAsync();
        }
        
        public bool SaveChanges()
        {
            int succeed = _dbContext.SaveChanges();
            return succeed > 0;
        }

        private bool SaveChanges(bool autoSave)
        {
            return !autoSave || SaveChanges();
        }

        private TEntity SaveChanges(TEntity entity, bool autoSave)
        {
            return SaveChanges(autoSave) ? entity : null;
        }

        private IEnumerable<TEntity> SaveChanges(IEnumerable<TEntity> entities, bool autoSave)
        {
            return SaveChanges(autoSave) ? entities : null;
        }

        public async Task<bool> SaveChangesAsync()
        {
            int succeed = await _dbContext.SaveChangesAsync();
            return succeed > 0;
        }

        private async Task<bool> SaveChangesAsync(bool autoSave)
        {
            if (autoSave)
                return await SaveChangesAsync();
            return true;
        }

        private async Task<TEntity> SaveChangesAsync(TEntity entity, bool autoSave)
        {
            if (await SaveChangesAsync(autoSave))
                return entity;
            return null;
        }

        private async Task<IEnumerable<TEntity>> SaveChangesAsync(IEnumerable<TEntity> entities, bool autoSave)
        {
            if (await SaveChangesAsync(autoSave))
                return entities;
            return null;
        }

        //Implement IDisposable.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                // Free other state (managed objects).
                _dbContext?.Dispose();
            }
            // Free your own state (unmanaged objects).
            // Set large fields to null.
            _utility = null;
            _dbSet = null;
            _dbContext = null;
            _disposed = true;
        }

        // Use C# destructor syntax for finalization code.
        ~Repository()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }
    }

    public class Repository<TEntity, TIEntity> : Repository<TEntity>, IRepository<TEntity, TIEntity>
        where TEntity : class, TIEntity
    {
        public Repository(IMyContext context, IUtility utility)
            : base(context, utility)
        {
        }
    }
}