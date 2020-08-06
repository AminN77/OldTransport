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
        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<Transporter> Transporters { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Accept> Accepts { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<SocialMedia> SocialMedias { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<ContactUs> ContactUs { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<HowItWorks> HowItWorks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasIndex(e => e.DestinationCountry);
                entity.HasIndex(e => e.DestinationCity);
                entity.HasIndex(e => e.BeginningCountry);
                entity.HasIndex(e => e.BeginningCity);

            });

            modelBuilder.Entity<Offer>(entity =>
            {
                entity.HasIndex(e => new { e.TransporterId, e.ProjectId }).IsUnique(true);
            });

            modelBuilder.Entity<Accept>(entity =>
            {
                entity.HasIndex(e => new { e.MerchantId, e.OfferId }).IsUnique(true);
            });

            modelBuilder.Entity<Project>().HasMany(b => b.Offers).WithOne(u => u.Project)
                .HasForeignKey(b => b.ProjectId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Merchant>().HasMany(b => b.Accepts).WithOne(u => u.Merchant)
                .HasForeignKey(b => b.MerchantId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transporter>().HasMany(t => t.Offers).WithOne(t => t.Transporter)
                .HasForeignKey(t => t.TransporterId).OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<Merchant>().HasMany(m => m.Projects).WithOne(m => m.Merchant)
            //    .HasForeignKey(m => m.MerchantId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Seed();
            base.OnModelCreating(modelBuilder);
        }
        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }
    }
}
