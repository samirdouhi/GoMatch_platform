using AuthService.DTos;

namespace AuthService.Interfaces
{
    public interface IUserService
    {
        public Task<Guid> RegisterAsync(RegisterRequestDto dto );
        Task<string> LoginAsync(LoginRequestDto dto, CancellationToken ct = default);
    }
}
