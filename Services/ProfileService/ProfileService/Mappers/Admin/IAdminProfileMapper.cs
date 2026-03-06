using ProfileService.DTOs.Admin;
using ProfileService.DTOs.Common;
using ProfileService.Models;

namespace ProfileService.Mappers.Admin;

public interface IAdminProfileMapper
{
    AdminProfileResponseDto ToResponseDto(AdminProfile profile);
    UpdateProfileResponseDto ToUpdateProfileResponseDto(AdminProfile profile);
    void MapCommonUpdates(UpdateProfileRequestDto dto, AdminProfile profile);
}