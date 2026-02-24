
using AuthService.DTOs;

namespace AuthService.Services;

public interface IAuthService
{
    Task<(bool Success, int Code, string? Erreur, RegisterResponseDto? Data)>
        RegisterAsync(RegisterRequestDto request, CancellationToken ct);

    Task<(bool Success, int Code, string? Erreur, LoginResponseDto? Data)>
      LoginAsync(LoginRequestDto dto, CancellationToken ct);

    Task<(bool Success, int Code, string? Erreur, RefreshResponseDto? Data)>
       RefreshAsync(RefreshRequestDto dto, CancellationToken ct);
    Task<(bool Success, int Code, string? Erreur)>
    LogoutAsync(string refreshToken, CancellationToken ct);
}