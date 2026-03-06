using ProfileService.Enums;

namespace ProfileService.DTOs.Commercant;

public sealed class CommercantProfileResponseDto
{
    public Guid UserId { get; set; }
    public string? Prenom { get; set; }
    public string? Nom { get; set; }
    public string? PhotoUrl { get; set; }
    public Langue Langue { get; set; }
    public string? Telephone { get; set; }
    public Guid? CommerceId { get; set; }
    public bool InscriptionTerminee { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
