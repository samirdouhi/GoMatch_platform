using AuthService.Enums;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options)
        {
        }

      
        public DbSet<User> Users { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(e =>
            {
                e.HasIndex(u => u.Email).IsUnique();

                e.Property(u => u.Email)
                    .HasMaxLength(256)
                    .IsRequired();

                e.Property(u => u.Role)
                    .HasConversion<string>()              // ✅ enum stocké en string
                    .HasMaxLength(30)
                    .HasDefaultValue(UserRole.Touriste)   // ✅ défaut côté DB
                    .IsRequired();

                // ✅ sécurité DB : interdit toute valeur hors liste
                e.ToTable(t => t.HasCheckConstraint(
                    "CK_Users_Role_Allowed",
                    "[Role] IN ('Touriste','Commercant','Admin')"
                ));
            });
        }
    }
}

