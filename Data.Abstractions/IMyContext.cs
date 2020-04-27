using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Data.Abstractions
{
    public interface IMyContext : IDisposable
    {
        int SaveChanges();
        Task<int> SaveChangesAsync();
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        //DbEntityEntry Entry(object entity);
        //DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    }
}
