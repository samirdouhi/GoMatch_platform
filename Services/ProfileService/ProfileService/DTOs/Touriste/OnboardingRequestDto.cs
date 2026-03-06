using System.ComponentModel.DataAnnotations;
using ProfileService.Enums;

namespace ProfileService.DTOs.Touriste;

public sealed class OnboardingRequestDto
{
    public List<string> Preferences { get; set; } = [];

    public BudgetRange? BudgetRange { get; set; }

    public DureeMoyenne? DureeMoyenne { get; set; }

    public TypeTouriste? TypeTouriste { get; set; }

    public Langue Langue { get; set; } = Langue.FR;

    [MaxLength(100)]
    public string? Nationalite { get; set; }

    public List<string> EquipesSuivies { get; set; } = [];
}