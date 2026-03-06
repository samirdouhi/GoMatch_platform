using Microsoft.EntityFrameworkCore;
using ProfileService.Models;

namespace ProfileService.Data;

public sealed class ProfileDbContext : DbContext
{
    public ProfileDbContext(DbContextOptions<ProfileDbContext> options)
        : base(options)
    {
    }

    // DbSet sur le type de base => EF gère tout l'héritage (TPH)
    public DbSet<ProfileBase> Profiles => Set<ProfileBase>();

    // (optionnel) DbSet typés : pratique pour requêtes, pas obligatoire
    public DbSet<TouristeProfile> Touristes => Set<TouristeProfile>();
    public DbSet<CommercantProfile> Commercants => Set<CommercantProfile>();
    public DbSet<AdminProfile> Admins => Set<AdminProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Bonne pratique : tout le mapping (TPH, indexes, conversions JSON, etc.)
        // est dans des classes IEntityTypeConfiguration<T> + scan automatique.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProfileDbContext).Assembly);
        // ApplyConfigurationsFromAssembly est le mécanisme standard pour garder un DbContext propre. :contentReference[oaicite:1]{index=1}
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyAuditInfo();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        ApplyAuditInfo();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ApplyAuditInfo()
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<ProfileBase>())
        {
            if (entry.State == EntityState.Added)
            {
                // Id : soit tu le génères dans ton service, soit ici.
                // Ici c'est OK et centralisé (couche data), mais tu peux aussi le faire côté application.
                if (entry.Entity.Id == Guid.Empty)
                    entry.Entity.Id = Guid.NewGuid();

                if (entry.Entity.CreatedAt == default)
                    entry.Entity.CreatedAt = utcNow;

                entry.Entity.UpdatedAt = utcNow;
                entry.Entity.IsActive = true; // tu peux retirer si tu veux laisser la valeur telle quelle
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = utcNow;

                // Empêche la modif accidentelle de CreatedAt
                entry.Property(x => x.CreatedAt).IsModified = false;
            }
        }
    }
}