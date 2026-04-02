using Microsoft.AspNetCore.Http;
using ProfileService.Dtos.Touriste;
using ProfileService.DTOs.Common;
using ProfileService.DTOs.Touriste;

namespace ProfileService.Services.Touriste;

public interface ITouristeProfileService
{
    Task<TouristeProfileResponseDto> GetMyProfileAsync(CancellationToken ct);

    Task<TouristeProfileResponseDto> InitProfileAsync(CancellationToken ct);

    Task<TouristeProfileResponseDto> RegisterInitAsync(
        RegisterInitProfileRequestDto request,
        CancellationToken ct);

    Task<UpdateProfileResponseDto> UpdateProfileAsync(
        UpdateProfileRequestDto request,
        CancellationToken ct);

    Task<OnboardingResponseDto> CompleteOnboardingAsync(
        OnboardingRequestDto request,
        CancellationToken ct);

    Task<PreferencesResponseDto> UpdatePreferencesAsync(
        UpdatePreferencesRequestDto request,
        CancellationToken ct);

    Task<PhotoUploadResponseDto> UploadPhotoAsync(IFormFile photo, CancellationToken ct);

    Task<(Stream Stream, string ContentType)> GetMyPhotoAsync(CancellationToken ct);
}