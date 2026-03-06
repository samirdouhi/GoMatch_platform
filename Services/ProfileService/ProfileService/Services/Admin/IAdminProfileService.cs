using ProfileService.DTOs.Admin;
using ProfileService.DTOs.Common;

namespace ProfileService.Services.Admin;

public interface IAdminProfileService
{
    Task<AdminProfileResponseDto> GetByUserIdAsync(Guid userId, CancellationToken ct);

    Task<AdminProfileResponseDto> InitProfileAsync(
        InitProfileRequestDto request,
        CancellationToken ct);

    Task<UpdateProfileResponseDto> UpdateProfileAsync(
        Guid userId,
        UpdateProfileRequestDto request,
        CancellationToken ct);
}