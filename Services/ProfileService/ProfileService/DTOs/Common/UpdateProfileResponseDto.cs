using ProfileService.Enums;

namespace ProfileService.DTOs.Common;

public sealed class UpdateProfileResponseDto
{
    public Guid UserId { get; set; }
    public string? Prenom { get; set; }
    public string? Nom { get; set; }
    public DateOnly? DateNaissance { get; set; }
    public Genre? Genre { get; set; }
    public string? PhotoUrl { get; set; }
    public Langue Langue { get; set; }
    public DateTime UpdatedAt { get; set; }
}