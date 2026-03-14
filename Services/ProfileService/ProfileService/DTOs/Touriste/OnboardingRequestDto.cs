using System.ComponentModel.DataAnnotations;
using ProfileService.Enums;

namespace ProfileService.DTOs.Touriste;

public sealed class OnboardingRequestDto
{
    public List<string> Preferences { get; set; } = [];

 
    public Langue Langue { get; set; } = Langue.FR;

    [MaxLength(100)]
    public string? Nationalite { get; set; }

    public List<string> EquipesSuivies { get; set; } = [];
}