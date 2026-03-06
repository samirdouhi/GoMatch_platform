using ProfileService.Enums;

namespace ProfileService.DTOs.Touriste;

public sealed class PreferencesResponseDto
{
    public Guid UserId { get; set; }
    public List<string> Preferences { get; set; } = [];
    public List<string> EquipesSuivies { get; set; } = [];
    public TypeTouriste? TypeTouriste { get; set; }
    public BudgetRange? BudgetRange { get; set; }
    public DureeMoyenne? DureeMoyenne { get; set; }
}