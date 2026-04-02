using ProfileService.DTOs.Commercant;
using ProfileService.DTOs.Common;
using ProfileService.Models;

namespace ProfileService.Mappers.Commercant;

public sealed class CommercantProfileMapper : ICommercantProfileMapper
{
    private const string MyPhotoEndpoint = "/api/commercant/profile/me/photo";

    private static string BuildPhotoUrl(CommercantProfile profile)
        => string.IsNullOrWhiteSpace(profile.PhotoPath)
            ? string.Empty
            : MyPhotoEndpoint;

    public CommercantProfileResponseDto ToResponseDto(CommercantProfile profile) => new()
    {
        UserId = profile.UserId,
        Prenom = profile.Prenom,
        Nom = profile.Nom,
        PhotoUrl = BuildPhotoUrl(profile),
        Langue = profile.Langue,
        Telephone = profile.Telephone,
        CommerceId = profile.CommerceId,
        InscriptionTerminee = profile.InscriptionTerminee,
        CreatedAt = profile.CreatedAt,
        UpdatedAt = profile.UpdatedAt
    };

    public UpdateProfileResponseDto ToUpdateProfileResponseDto(CommercantProfile profile) => new()
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

    public void MapRequest(CommercantProfileRequestDto dto, CommercantProfile profile)
    {
        profile.Telephone = dto.Telephone;
        profile.CommerceId = dto.CommerceId;
    }

    public void MapCommonUpdates(UpdateProfileRequestDto dto, CommercantProfile profile)
    {
        profile.Prenom = dto.Prenom;
        profile.Nom = dto.Nom;
        profile.DateNaissance = dto.DateNaissance;
        profile.Genre = dto.Genre;

        if (dto.Langue.HasValue)
            profile.Langue = dto.Langue.Value;
    }
}