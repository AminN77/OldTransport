using System.Text.RegularExpressions;
using Data.Abstractions;
using Data.Model;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Data.DataAccess.Context
{
    public class SqlServerDbContext : DbContext, IMyContext
    {
        public SqlServerDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.FirstName);
            modelBuilder.Entity<User>().HasIndex(u => u.LastName);
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.EmailAddress).IsUnique();

            modelBuilder.Seed();
            base.OnModelCreating(modelBuilder);
        }
        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }
    }
}
