using ProfileService.DTOs.Common;
using ProfileService.DTOs.Touriste;
using ProfileService.Models;

namespace ProfileService.Mappers.Touriste;

public interface ITouristeProfileMapper
{
    OnboardingResponseDto ToOnboardingResponseDto(TouristeProfile profile);
    TouristeProfileResponseDto ToProfileResponseDto(TouristeProfile profile);
    PreferencesResponseDto ToPreferencesResponseDto(TouristeProfile profile);
    UpdateProfileResponseDto ToUpdateProfileResponseDto(TouristeProfile profile);

    void MapCommonUpdates(UpdateProfileRequestDto dto, TouristeProfile profile);
    void MapOnboarding(OnboardingRequestDto dto, TouristeProfile profile);
    void MapPreferences(UpdatePreferencesRequestDto dto, TouristeProfile profile);
}
