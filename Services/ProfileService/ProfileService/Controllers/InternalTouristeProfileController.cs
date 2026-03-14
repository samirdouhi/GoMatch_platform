using Microsoft.AspNetCore.Mvc;
using ProfileService.Dtos.Touriste;
using ProfileService.DTOs.Touriste;
using ProfileService.Services.Touriste;

namespace ProfileService.Controllers.Touriste;

[ApiController]
[Route("internal/touriste/profile")]
public class InternalTouristeProfileController : ControllerBase
{
    private readonly ITouristeProfileService _touristeProfileService;

    public InternalTouristeProfileController(ITouristeProfileService touristeProfileService)
    {
        _touristeProfileService = touristeProfileService;
    }

    [HttpPost("register-init")]
    public async Task<IActionResult> RegisterInit(
        [FromBody] RegisterInitProfileRequestDto request,
        CancellationToken ct)
    {
        var result = await _touristeProfileService.RegisterInitAsync(request, ct);
        return Ok(result);
    }
}