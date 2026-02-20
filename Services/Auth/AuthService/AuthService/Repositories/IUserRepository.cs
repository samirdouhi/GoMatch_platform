using AuthService.Models;

namespace AuthService.Repositories;

public interface IUserRepository
{
    Task<bool> EmailExisteAsync(string emailNormalise, CancellationToken ct);
    Task<User?> GetByEmailAsync(string emailNormalise, CancellationToken ct);
    Task AjouterAsync(User utilisateur, CancellationToken ct);
    Task SauvegarderAsync(CancellationToken ct);
}