using ProfileService.Enums;

namespace ProfileService.DTOs.Touriste;

public sealed class OnboardingResponseDto
{
    public Guid UserId { get; set; }
    public Langue Langue { get; set; }
    public string? Nationalite { get; set; }
    public List<string> Preferences { get; set; } = [];
    public BudgetRange? BudgetRange { get; set; }
    public DureeMoyenne? DureeMoyenne { get; set; }
    public TypeTouriste? TypeTouriste { get; set; }
    public List<string> EquipesSuivies { get; set; } = [];
    public bool InscriptionTerminee { get; set; }
}