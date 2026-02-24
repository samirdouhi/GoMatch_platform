using AuthService.DTOs;
using AuthService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request, CancellationToken ct)
    {
        // ✅ Grâce à [ApiController] + DataAnnotations sur RegisterRequestDto :
        // - Email invalide => 400 automatiquement
        // - Password trop court => 400 automatiquement

        var (success, code, erreur, data) = await _service.RegisterAsync(request, ct);

        if (!success)
        {
            // 400 (mot de passe faible) ou 409 (email déjà utilisé)
            return StatusCode(code, new { erreur });
        }

        // ✅ 201 : réponse sécurisée
        return StatusCode(201, data);
    }
    // POST /auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto, CancellationToken ct)
    {
        var (success, code, erreur, data) = await _service.LoginAsync(dto, ct);

        if (!success)
            return StatusCode(code, new { message = erreur });

        // ✅ 200 OK + token
        return Ok(data);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
    [FromBody] RefreshRequestDto dto,
    CancellationToken ct)
    {
        var (success, code, erreur) =
            await _service.LogoutAsync(dto.RefreshToken, ct);

        if (!success)
            return StatusCode(code, new { erreur });

        return Ok(new { message = "Déconnexion réussie." });
    }
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(User.Claims.Select(c => new
        {
            c.Type,
            c.Value
        }));
    }
    [Authorize(Roles = "Admin")]
    [HttpGet("admin-test")]
    public IActionResult AdminTest()
    {
        return Ok("Admin OK");
    }
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto, CancellationToken ct)
    {
        var (success, code, erreur, data) = await _service.RefreshAsync(dto, ct);
        if (!success) return StatusCode(code, new { erreur });
        return Ok(data);
    }
    [HttpGet("boom")]
    public IActionResult Boom() => throw new Exception("BOOM TEST");
}
