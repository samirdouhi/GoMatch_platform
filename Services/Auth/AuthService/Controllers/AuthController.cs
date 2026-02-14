using AuthService.DTos;
using AuthService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _authService;

        // Injection de dépendance via l’interface
        public AuthController(IUserService authService)
        {
            _authService = authService;
        }
        [HttpGet("test")]
        public IActionResult Test() => Ok("Auth OK");

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequestDto dto,
            CancellationToken ct)
        {
            var userId = await _authService.RegisterAsync(dto);
            return Ok(new { userId });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequestDto dto,
            CancellationToken ct)
        {
            var token = await _authService.LoginAsync(dto, ct);
            return Ok(new { token });
        }
    }
}
