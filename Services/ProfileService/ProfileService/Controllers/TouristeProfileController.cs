using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProfileService.DTOs.Common;
using ProfileService.DTOs.Touriste;
using ProfileService.Services.Touriste;

namespace ProfileService.Controllers;

[Authorize]
[ApiController]
[Route("api/touriste/profile")]
public sealed class TouristeProfileController : ControllerBase
{
    private readonly ITouristeProfileService _service;

    public TouristeProfileController(ITouristeProfileService service)
    {
        _service = service;
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(TouristeProfileResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<TouristeProfileResponseDto>> GetMyProfile(CancellationToken ct)
    {
        var response = await _service.GetMyProfileAsync(ct);
        return Ok(response);
    }

    [HttpPost("me/init")]
    [ProducesResponseType(typeof(TouristeProfileResponseDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<TouristeProfileResponseDto>> InitProfile(CancellationToken ct)
    {
        var response = await _service.InitProfileAsync(ct);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("me")]
    [ProducesResponseType(typeof(UpdateProfileResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UpdateProfileResponseDto>> UpdateProfile(
        [FromBody] UpdateProfileRequestDto request,
        CancellationToken ct)
    {
        var response = await _service.UpdateProfileAsync(request, ct);
        return Ok(response);
    }

    [HttpPost("me/onboarding")]
    [ProducesResponseType(typeof(OnboardingResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<OnboardingResponseDto>> CompleteOnboarding(
        [FromBody] OnboardingRequestDto request,
        CancellationToken ct)
    {
        var response = await _service.CompleteOnboardingAsync(request, ct);
        return Ok(response);
    }

    [HttpPut("me/preferences")]
    [ProducesResponseType(typeof(PreferencesResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PreferencesResponseDto>> UpdatePreferences(
        [FromBody] UpdatePreferencesRequestDto request,
        CancellationToken ct)
    {
        var response = await _service.UpdatePreferencesAsync(request, ct);
        return Ok(response);
    }
}