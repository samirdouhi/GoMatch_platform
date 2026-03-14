using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProfileService.DTOs.Admin;
using ProfileService.DTOs.Common;
using ProfileService.Services.Admin;

namespace ProfileService.Controllers;

[Authorize]
[ApiController]
[Route("api/admin/profile")]
public sealed class AdminProfileController : ControllerBase
{
    private readonly IAdminProfileService _service;

    public AdminProfileController(IAdminProfileService service)
    {
        _service = service;
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(AdminProfileResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AdminProfileResponseDto>> GetMyProfile(CancellationToken ct)
    {
        var response = await _service.GetMyProfileAsync(ct);
        return Ok(response);
    }

    [HttpPost("me/init")]
    [ProducesResponseType(typeof(AdminProfileResponseDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<AdminProfileResponseDto>> InitProfile(CancellationToken ct)
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
}
