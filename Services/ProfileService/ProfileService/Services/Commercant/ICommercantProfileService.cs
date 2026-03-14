using ProfileService.DTOs.Commercant;
using ProfileService.DTOs.Common;

namespace ProfileService.Services.Commercant;

public interface ICommercantProfileService
{
    Task<CommercantProfileResponseDto> GetMyProfileAsync(CancellationToken ct);

    Task<CommercantProfileResponseDto> InitProfileAsync(CancellationToken ct);

    Task<CommercantProfileResponseDto> CompleteProfileAsync(
        CommercantProfileRequestDto request,
        CancellationToken ct);

    Task<UpdateProfileResponseDto> UpdateProfileAsync(
        UpdateProfileRequestDto request,
        CancellationToken ct);
}