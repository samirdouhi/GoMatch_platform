using ProfileService.DTOs.Common;
using ProfileService.DTOs.Touriste;
using ProfileService.Models;

namespace ProfileService.Mappers.Touriste;

public sealed class TouristeProfileMapper : ITouristeProfileMapper
{
    private const string MyPhotoEndpoint = "/profile/me/photo";

    private static string BuildPhotoUrl(TouristeProfile profile)
        => string.IsNullOrWhiteSpace(profile.PhotoPath)
            ? string.Empty
            : MyPhotoEndpoint;

    public OnboardingResponseDto ToOnboardingResponseDto(TouristeProfile profile) => new()
    {
        UserId = profile.UserId,
        Langue = profile.Langue,
        Nationalite = profile.Nationalite,
        Preferences = profile.Preferences?.ToList() ?? [],
        EquipesSuivies = profile.EquipesSuivies?.ToList() ?? [],
        InscriptionTerminee = profile.InscriptionTerminee
    };

    public TouristeProfileResponseDto ToProfileResponseDto(TouristeProfile profile) => new()
    {
        UserId = profile.UserId,
        Prenom = profile.Prenom,
        Nom = profile.Nom,
        DateNaissance = profile.DateNaissance,
        Genre = profile.Genre,
        PhotoUrl = BuildPhotoUrl(profile),
        Langue = profile.Langue,
        Nationalite = profile.Nationalite,
        Preferences = profile.Preferences?.ToList() ?? [],
        EquipesSuivies = profile.EquipesSuivies?.ToList() ?? [],
        InscriptionTerminee = profile.InscriptionTerminee,
        CreatedAt = profile.CreatedAt,
        UpdatedAt = profile.UpdatedAt
    };

    public PreferencesResponseDto ToPreferencesResponseDto(TouristeProfile profile) => new()
    {
        UserId = profile.UserId,
        Preferences = profile.Preferences?.ToList() ?? [],
        EquipesSuivies = profile.EquipesSuivies?.ToList() ?? [],
    };

    public UpdateProfileResponseDto ToUpdateProfileResponseDto(TouristeProfile profile) => new()
    {
        UserId = profile.UserId,
        Prenom = profile.Prenom,
        Nom = profile.Nom,
        DateNaissance = profile.DateNaissance,
        Genre = profile.Genre,
        PhotoUrl = BuildPhotoUrl(profile),
        Langue = profile.Langue,
        UpdatedAt = profile.UpdatedAt
    };

    public void MapCommonUpdates(UpdateProfileRequestDto dto, TouristeProfile profile)
    {
        profile.Prenom = dto.Prenom;
        profile.Nom = dto.Nom;
        profile.DateNaissance = dto.DateNaissance;
        profile.Genre = dto.Genre;

        if (dto.Langue.HasValue)
            profile.Langue = dto.Langue.Value;
    }

    public void MapOnboarding(OnboardingRequestDto dto, TouristeProfile profile)
    {
        profile.Preferences = dto.Preferences;
        profile.Langue = dto.Langue;
        profile.EquipesSuivies = dto.EquipesSuivies;
    }

    public void MapPreferences(UpdatePreferencesRequestDto request, TouristeProfile profile)
    {
        profile.Preferences = request.Preferences;
        profile.EquipesSuivies = request.EquipesSuivies;

        if (request.Langue.HasValue)
            profile.Langue = request.Langue.Value;
    }
}