using AuthService.Enums;

namespace AuthService.DTOs;

public sealed class RegisterResponseDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = default!;
    public UserRole Role { get; init; }
}