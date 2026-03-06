using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProfileService.Models;
using System.Linq;
using System.Text.Json;

namespace ProfileService.Data.Configurations;

public sealed class TouristeProfileConfiguration : IEntityTypeConfiguration<TouristeProfile>
{
    public void Configure(EntityTypeBuilder<TouristeProfile> b)
    {
        b.Property(x => x.Nationalite).HasMaxLength(100);

        var jsonOptions = new JsonSerializerOptions();

        var listToJsonConverter = new ValueConverter<List<string>, string>(
            v => JsonSerializer.Serialize(v, jsonOptions),
            v => string.IsNullOrWhiteSpace(v)
                ? new List<string>()
                : (JsonSerializer.Deserialize<List<string>>(v, jsonOptions) ?? new List<string>())
        );

        var listComparer = new ValueComparer<List<string>>(
            (a, c) => (a ?? new List<string>()).SequenceEqual(c ?? new List<string>()),
            v => (v ?? new List<string>()).Aggregate(0, (h, x) => HashCode.Combine(h, x == null ? 0 : x.GetHashCode())),
            v => (v ?? new List<string>()).ToList()
        );

        b.Property(x => x.Preferences)
            .HasConversion(listToJsonConverter)
            .Metadata.SetValueComparer(listComparer);

        b.Property(x => x.EquipesSuivies)
            .HasConversion(listToJsonConverter)
            .Metadata.SetValueComparer(listComparer);
    }
}