using ProfileService.DTOs.Commercant;
using ProfileService.DTOs.Common;

namespace ProfileService.Services.Commercant;

public interface ICommercantProfileService
{
    Task<CommercantProfileResponseDto> GetByUserIdAsync(Guid userId, CancellationToken ct);

    Task<CommercantProfileResponseDto> InitProfileAsync(
        InitProfileRequestDto request,
        CancellationToken ct);

    Task<CommercantProfileResponseDto> CompleteProfileAsync(
        Guid userId,
        CommercantProfileRequestDto request,
        CancellationToken ct);

    Task<UpdateProfileResponseDto> UpdateProfileAsync(
        Guid userId,
        UpdateProfileRequestDto request,
        CancellationToken ct);
}