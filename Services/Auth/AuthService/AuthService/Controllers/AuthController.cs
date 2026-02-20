using AuthService.DTOs;
using AuthService.Services;
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
}
