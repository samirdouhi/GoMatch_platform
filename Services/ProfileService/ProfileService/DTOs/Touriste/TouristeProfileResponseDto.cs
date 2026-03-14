using ProfileService.Enums;

namespace ProfileService.DTOs.Touriste;

public sealed class TouristeProfileResponseDto
{
    public Guid UserId { get; set; }
    public string? Prenom { get; set; }
    public string? Nom { get; set; }
    public DateOnly? DateNaissance { get; set; }
    public Genre? Genre { get; set; }
    public string? PhotoUrl { get; set; }
    public Langue Langue { get; set; }
    public string? Nationalite { get; set; }
    public List<string> Preferences { get; set; } = [];
  
    public List<string> EquipesSuivies { get; set; } = [];
    public bool InscriptionTerminee { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}