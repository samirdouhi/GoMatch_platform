using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProfileService.DTOs.Commercant;
using ProfileService.DTOs.Common;
using ProfileService.Services.Commercant;

namespace ProfileService.Controllers;

[Authorize]
[ApiController]
[Route("api/commercant/profile")]
public sealed class CommercantProfileController : ControllerBase
{
    private readonly ICommercantProfileService _service;

    public CommercantProfileController(ICommercantProfileService service)
    {
        _service = service;
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(CommercantProfileResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CommercantProfileResponseDto>> GetMyProfile(CancellationToken ct)
    {
        var response = await _service.GetMyProfileAsync(ct);
        return Ok(response);
    }

    [HttpPost("me/init")]
    [ProducesResponseType(typeof(CommercantProfileResponseDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<CommercantProfileResponseDto>> InitProfile(CancellationToken ct)
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

    [HttpPost("me/complete")]
    [ProducesResponseType(typeof(CommercantProfileResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CommercantProfileResponseDto>> CompleteProfile(
        [FromBody] CommercantProfileRequestDto request,
        CancellationToken ct)
    {
        var response = await _service.CompleteProfileAsync(request, ct);
        return Ok(response);
    }
}