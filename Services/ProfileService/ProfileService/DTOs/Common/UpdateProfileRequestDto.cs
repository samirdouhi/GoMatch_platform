using System.ComponentModel.DataAnnotations;
using ProfileService.Enums;

namespace ProfileService.DTOs.Common;

public sealed class UpdateProfileRequestDto
{
    [MaxLength(100)]
    [RegularExpression(@"^[a-zA-ZÀ-ÿ\s\-]+$")]
    public string? Prenom { get; set; }

    [MaxLength(100)]
    [RegularExpression(@"^[a-zA-ZÀ-ÿ\s\-]+$")]
    public string? Nom { get; set; }

    public DateOnly? DateNaissance { get; set; }

    public Genre? Genre { get; set; }

    [Url]
    [MaxLength(512)]
    public string? PhotoUrl { get; set; }

    public Langue? Langue { get; set; }
}