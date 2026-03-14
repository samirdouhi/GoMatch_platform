using ProfileService.Enums;

namespace ProfileService.DTOs.Touriste;

public sealed class PreferencesResponseDto
{
    public Guid UserId { get; set; }
    public List<string> Preferences { get; set; } = [];
    public List<string> EquipesSuivies { get; set; } = [];
   
}