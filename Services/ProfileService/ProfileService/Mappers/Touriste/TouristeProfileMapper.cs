using ProfileService.DTOs.Common;
using ProfileService.DTOs.Touriste;
using ProfileService.Models;

namespace ProfileService.Mappers.Touriste;

public sealed class TouristeProfileMapper : ITouristeProfileMapper
{
    public OnboardingResponseDto ToOnboardingResponseDto(TouristeProfile profile) => new()
    {
        UserId = profile.UserId,
        Langue = profile.Langue,
        Nationalite = profile.Nationalite,
        Preferences = profile.Preferences?.ToList() ?? [],
        BudgetRange = profile.BudgetRange,
        DureeMoyenne = profile.DureeMoyenne,
        TypeTouriste = profile.TypeTouriste,
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
        PhotoUrl = profile.PhotoUrl,
        Langue = profile.Langue,
        Nationalite = profile.Nationalite,
        Preferences = profile.Preferences?.ToList() ?? [],
        BudgetRange = profile.BudgetRange,
        DureeMoyenne = profile.DureeMoyenne,
        TypeTouriste = profile.TypeTouriste,
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
        TypeTouriste = profile.TypeTouriste,
        BudgetRange = profile.BudgetRange,
        DureeMoyenne = profile.DureeMoyenne
    };

    public UpdateProfileResponseDto ToUpdateProfileResponseDto(TouristeProfile profile) => new()
    {
        UserId = profile.UserId,
        Prenom = profile.Prenom,
        Nom = profile.Nom,
        DateNaissance = profile.DateNaissance,
        Genre = profile.Genre,
        PhotoUrl = profile.PhotoUrl,
        Langue = profile.Langue,
        UpdatedAt = profile.UpdatedAt
    };

    public void MapCommonUpdates(UpdateProfileRequestDto dto, TouristeProfile profile)
    {
        profile.Prenom = dto.Prenom;
        profile.Nom = dto.Nom;
        profile.DateNaissance = dto.DateNaissance;
        profile.Genre = dto.Genre;
        profile.PhotoUrl = dto.PhotoUrl;
        if (dto.Langue.HasValue)
            profile.Langue = dto.Langue.Value;
    }

    public void MapOnboarding(OnboardingRequestDto dto, TouristeProfile profile)
    {
        profile.Preferences = dto.Preferences;
        profile.BudgetRange = dto.BudgetRange;
        profile.DureeMoyenne = dto.DureeMoyenne;
        profile.TypeTouriste = dto.TypeTouriste;
        profile.Langue = dto.Langue;
        profile.Nationalite = dto.Nationalite;
        profile.EquipesSuivies = dto.EquipesSuivies;
    }

    public void MapPreferences(UpdatePreferencesRequestDto dto, TouristeProfile profile)
    {
        profile.Preferences = dto.Preferences;
        profile.EquipesSuivies = dto.EquipesSuivies;
    }
}