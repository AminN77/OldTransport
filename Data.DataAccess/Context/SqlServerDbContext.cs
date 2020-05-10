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
        public DbSet<Role> Roles { set; get; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<User>().HasIndex(u => u.Name);
            //modelBuilder.Entity<User>().HasIndex(u => u.EmailAddress).IsUnique();

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.EmailAddress).IsUnique();
                entity.Property(e => e.Password).IsRequired();
                entity.Property(e => e.SerialNumber).HasMaxLength(450);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(450).IsRequired();
                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.RoleId);
                entity.Property(e => e.UserId);
                entity.Property(e => e.RoleId);
                //entity.HasOne(d => d.Role).WithMany(p => p.UserRoles).HasForeignKey(d => d.RoleId);
                //entity.HasOne(d => d.User).WithMany(p => p.UserRoles).HasForeignKey(d => d.UserId);
            });

            modelBuilder.Seed();
            base.OnModelCreating(modelBuilder);
        }
        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }
    }
}
