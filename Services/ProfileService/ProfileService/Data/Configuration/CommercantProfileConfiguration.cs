using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProfileService.Models;

namespace ProfileService.Data.Configurations;

public sealed class CommercantProfileConfiguration : IEntityTypeConfiguration<CommercantProfile>
{
    public void Configure(EntityTypeBuilder<CommercantProfile> b)
    {
        b.Property(x => x.Telephone).HasMaxLength(20);
    }
}
