
using AuthService.DTOs;

namespace AuthService.Services;

public interface IAuthService
{
    Task<(bool Success, int Code, string? Erreur, RegisterResponseDto? Data)>
        RegisterAsync(RegisterRequestDto request, CancellationToken ct);
}