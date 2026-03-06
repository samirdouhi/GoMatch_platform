using ProfileService.DTOs.Commercant;
using ProfileService.DTOs.Common;
using ProfileService.Models;

namespace ProfileService.Mappers.Commercant;

public interface ICommercantProfileMapper
{
    CommercantProfileResponseDto ToResponseDto(CommercantProfile profile);
    UpdateProfileResponseDto ToUpdateProfileResponseDto(CommercantProfile profile);
    void MapRequest(CommercantProfileRequestDto dto, CommercantProfile profile);
    void MapCommonUpdates(UpdateProfileRequestDto dto, CommercantProfile profile);
}