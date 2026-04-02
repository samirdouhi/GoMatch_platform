using ProfileService.DTOs.Admin;
using ProfileService.DTOs.Common;
using ProfileService.Models;

namespace ProfileService.Mappers.Admin;

public sealed class AdminProfileMapper : IAdminProfileMapper
{
    private const string MyPhotoEndpoint = "/api/admin/profile/me/photo";

    private static string BuildPhotoUrl(AdminProfile profile)
        => string.IsNullOrWhiteSpace(profile.PhotoPath)
            ? string.Empty
            : MyPhotoEndpoint;

    public AdminProfileResponseDto ToResponseDto(AdminProfile profile) => new()
    {
        UserId = profile.UserId,
        Prenom = profile.Prenom,
        Nom = profile.Nom,
        PhotoUrl = BuildPhotoUrl(profile),
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
        PhotoUrl = BuildPhotoUrl(profile),
        Langue = profile.Langue,
        UpdatedAt = profile.UpdatedAt
    };

    public void MapCommonUpdates(UpdateProfileRequestDto dto, AdminProfile profile)
    {
        profile.Prenom = dto.Prenom;
        profile.Nom = dto.Nom;
        profile.DateNaissance = dto.DateNaissance;
        profile.Genre = dto.Genre;

        if (dto.Langue.HasValue)
            profile.Langue = dto.Langue.Value;
    }
}