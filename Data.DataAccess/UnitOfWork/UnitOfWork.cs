using System.Collections.Generic;
using Data.DataAccess.Repository;
using Data.Abstractions;
using System;
using Cross.Abstractions;

namespace Data.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IMyContext _context;
        private readonly IDictionary<Type, object> _repositories;
        private readonly IUtility _utility;

        public UnitOfWork(IMyContext context, IUtility utility)
        {
            _context = context;
            _repositories = new Dictionary<Type, object>();
            _utility = utility;
        }

        public IRepository<TEntity> GetRepository<TEntity>()
            where TEntity : class
        {
            Type entityType = typeof(TEntity);
            if (_repositories.Keys.Contains(entityType) == true)
            {
                return _repositories[entityType] as IRepository<TEntity>;
            }
            IRepository<TEntity> repository = new Repository<TEntity>(_context, _utility);
            _repositories.Add(entityType, repository);
            return repository;
        }

        public void Dispose()
        {
            ;
        }
    }
}
