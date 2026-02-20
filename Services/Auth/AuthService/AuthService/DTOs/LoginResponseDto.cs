public sealed class LoginResponseDto
{
    public string AccessToken { get; set; } = default!;
    public DateTime ExpiresAtUtc { get; set; }
}