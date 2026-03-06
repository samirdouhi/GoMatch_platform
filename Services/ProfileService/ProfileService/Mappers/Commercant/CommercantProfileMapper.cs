using ProfileService.DTOs.Commercant;
using ProfileService.DTOs.Common;
using ProfileService.Models;

namespace ProfileService.Mappers.Commercant;

public sealed class CommercantProfileMapper : ICommercantProfileMapper
{
    public CommercantProfileResponseDto ToResponseDto(CommercantProfile profile) => new()
    {
        UserId = profile.UserId,
        Prenom = profile.Prenom,
        Nom = profile.Nom,
        PhotoUrl = profile.PhotoUrl,
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
        PhotoUrl = profile.PhotoUrl,
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
        profile.PhotoUrl = dto.PhotoUrl;
        if (dto.Langue.HasValue)
            profile.Langue = dto.Langue.Value;
    }
}