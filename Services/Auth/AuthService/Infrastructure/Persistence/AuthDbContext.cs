using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Persistence
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Email)
                      .IsRequired()
                      .HasMaxLength(256);

                entity.HasIndex(x => x.Email)
                      .IsUnique();

                entity.Property(x => x.PasswordHash)
                      .IsRequired();

                entity.Property(x => x.Role)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(x => x.IsActive)
                      .IsRequired();

                entity.Property(x => x.IsEmailVerified)
                      .IsRequired();

                entity.Property(x => x.CreatedAtUtc)
                      .IsRequired();

                entity.Property(x => x.LastLoginAtUtc);
            });
        }
    }
}
