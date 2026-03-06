using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProfileService.Models;

namespace ProfileService.Data.Configurations;

public sealed class AdminProfileConfiguration : IEntityTypeConfiguration<AdminProfile>
{
    public void Configure(EntityTypeBuilder<AdminProfile> b)
    {
        // Champs spécifiques Admin
        b.Property(x => x.Departement)
            .HasMaxLength(100);

        // Optionnel (propre) : index si tu filtres souvent par département
         b.HasIndex(x => x.Departement);
    }
}