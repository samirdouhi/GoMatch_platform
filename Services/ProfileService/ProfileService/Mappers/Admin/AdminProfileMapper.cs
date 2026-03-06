using ProfileService.DTOs.Admin;
using ProfileService.DTOs.Common;
using ProfileService.Models;

namespace ProfileService.Mappers.Admin;

public sealed class AdminProfileMapper : IAdminProfileMapper
{
    public AdminProfileResponseDto ToResponseDto(AdminProfile profile) => new()
    {
        UserId = profile.UserId,
        Prenom = profile.Prenom,
        Nom = profile.Nom,
        PhotoUrl = profile.PhotoUrl,
        Langue = profile.Langue,
        Departement = profile.Departement,
        InscriptionTerminee = profile.InscriptionTerminee,
        CreatedAt = profile.CreatedAt,
        UpdatedAt = profile.UpdatedAt
    };

    public UpdateProfileResponseDto ToUpdateProfileResponseDto(AdminProfile profile) => new()
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

    public void MapCommonUpdates(UpdateProfileRequestDto dto, AdminProfile profile)
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