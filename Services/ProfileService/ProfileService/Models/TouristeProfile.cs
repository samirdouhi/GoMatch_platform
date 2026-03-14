using ProfileService.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProfileService.Models;

public class TouristeProfile : ProfileBase
{
    [MaxLength(100)]
    public string? Nationalite { get; set; }

    public List<string> Preferences { get; set; } = [];

    public List<string> EquipesSuivies { get; set; } = [];
}