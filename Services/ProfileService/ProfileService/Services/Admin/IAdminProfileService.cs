using ProfileService.DTOs.Admin;
using ProfileService.DTOs.Common;

namespace ProfileService.Services.Admin;

public interface IAdminProfileService
{
    Task<AdminProfileResponseDto> GetMyProfileAsync(CancellationToken ct);

    Task<AdminProfileResponseDto> InitProfileAsync(CancellationToken ct);

    Task<UpdateProfileResponseDto> UpdateProfileAsync(
        UpdateProfileRequestDto request,
        CancellationToken ct);
}