using ProfileService.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProfileService.DTOs.Touriste;

public sealed class UpdatePreferencesRequestDto
{
    public List<string> Preferences { get; set; } = [];
    public List<string> EquipesSuivies { get; set; } = [];

   

    [MaxLength(100)]
    public string? Nationalite { get; set; }

    public Langue? Langue { get; set; }
}