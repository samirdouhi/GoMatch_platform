using ProfileService.DTOs.Common;
using ProfileService.DTOs.Touriste;

namespace ProfileService.Services.Touriste;

public interface ITouristeProfileService
{
    Task<TouristeProfileResponseDto> GetByUserIdAsync(Guid userId, CancellationToken ct);

    Task<TouristeProfileResponseDto> InitProfileAsync(
        InitProfileRequestDto request,
        CancellationToken ct);

    Task<UpdateProfileResponseDto> UpdateProfileAsync(
        Guid userId,
        UpdateProfileRequestDto request,
        CancellationToken ct);

    Task<OnboardingResponseDto> CompleteOnboardingAsync(
        Guid userId,
        OnboardingRequestDto request,
        CancellationToken ct);

    Task<PreferencesResponseDto> UpdatePreferencesAsync(
        Guid userId,
        UpdatePreferencesRequestDto request,
        CancellationToken ct);
}