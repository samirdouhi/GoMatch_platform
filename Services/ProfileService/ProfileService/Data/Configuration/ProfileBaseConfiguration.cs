using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProfileService.Models;

namespace ProfileService.Data.Configurations;

public sealed class ProfileBaseConfiguration : IEntityTypeConfiguration<ProfileBase>
{
    public void Configure(EntityTypeBuilder<ProfileBase> b)
    {
        b.ToTable("Profiles");
        b.HasKey(x => x.Id);

        // TPH discriminator
        b.HasDiscriminator<string>("ProfileType")
            .HasValue<TouristeProfile>("Touriste")
            .HasValue<CommercantProfile>("Commercant")
            .HasValue<AdminProfile>("Admin");

        // 1 profil par user
        b.HasIndex(x => x.UserId).IsUnique();

        // Defaults DB UTC
        b.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        b.Property(x => x.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        b.Property(x => x.IsActive).HasDefaultValue(true);
    }
}