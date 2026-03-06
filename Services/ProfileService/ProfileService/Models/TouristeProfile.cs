using ProfileService.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProfileService.Models;

public class TouristeProfile : ProfileBase
{
    [MaxLength(100)]
    public string? Nationalite { get; set; }

    public List<string> Preferences { get; set; } = [];

    public BudgetRange? BudgetRange { get; set; }

    public DureeMoyenne? DureeMoyenne { get; set; }

    public TypeTouriste? TypeTouriste { get; set; }

    public List<string> EquipesSuivies { get; set; } = [];
}