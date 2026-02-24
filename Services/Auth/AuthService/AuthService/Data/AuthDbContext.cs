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
        public DbSet<RefreshToken> RefreshTokens { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(e =>
            {
                e.HasIndex(u => u.Email).IsUnique();

                e.Property(u => u.Email)
                    .HasMaxLength(256)
                    .IsRequired();

                e.Property(u => u.Role)
       .HasConversion<string>()
       .HasMaxLength(30)
       .HasDefaultValue(UserRole.Touriste)   // ✅ enum, pas string
       .IsRequired();

                e.ToTable(t => t.HasCheckConstraint(
                    "CK_Users_Role_Allowed",
                    "[Role] IN ('Touriste','Commercant','Admin')"
                ));
            });

            modelBuilder.Entity<RefreshToken>(e =>
            {
                e.HasKey(rt => rt.Id);

                e.Property(rt => rt.Token)
                    .HasMaxLength(512)   // ✅ marge sécurité
                    .IsRequired();

                e.HasIndex(rt => rt.Token).IsUnique();

                e.Property(rt => rt.CreatedAtUtc).IsRequired();
                e.Property(rt => rt.ExpiresAtUtc).IsRequired();

                e.Property(rt => rt.RevokedAtUtc);
                e.Property(rt => rt.ReplacedByToken).HasMaxLength(512);

                e.HasOne(rt => rt.User)
                    .WithMany() // ou WithMany(u => u.RefreshTokens) si tu ajoutes la nav
                    .HasForeignKey(rt => rt.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}

