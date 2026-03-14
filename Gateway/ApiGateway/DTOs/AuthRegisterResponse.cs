namespace ApiGateway.DTOs;

public class AuthRegisterResponse
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = default!;
    public string Message { get; set; } = default!;
}