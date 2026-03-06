namespace ProfileService.DTOs.Touriste;

public sealed class UpdatePreferencesRequestDto
{
    public List<string> Preferences { get; set; } = [];
    public List<string> EquipesSuivies { get; set; } = [];
}