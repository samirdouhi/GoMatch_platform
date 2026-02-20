
using AuthService.DTOs;
using AuthService.Enums;
using AuthService.Models;
using AuthService.Repositories;
using AuthService.Security;

namespace AuthService.Services;

public sealed class AuthentificationService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly PasswordPolicy _passwordPolicy;

    public AuthentificationService(
        IUserRepository users,
        IPasswordHasher passwordHasher,
        PasswordPolicy passwordPolicy)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _passwordPolicy = passwordPolicy;
    }

    public async Task<(bool Success, int Code, string? Erreur, RegisterResponseDto? Data)>
        RegisterAsync(RegisterRequestDto request, CancellationToken ct)
    {
        // 1️⃣ Normaliser email
        var email = (request.Email ?? string.Empty)
            .Trim()
            .ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(email))
            return (false, 400, "Email obligatoire.", null);

        // 2️⃣ Vérifier mot de passe fort
        if (!_passwordPolicy.EstValide(request.Password, out var erreurPwd))
            return (false, 400, erreurPwd, null);

        // 3️⃣ Vérifier email déjà utilisé
        if (await _users.EmailExisteAsync(email, ct))
            return (false, 409, "Email déjà utilisé.", null);

        // 4️⃣ Créer utilisateur (Role forcé Touriste)
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = _passwordHasher.Hasher(request.Password),
            Role = UserRole.Touriste,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // 5️⃣ Sauvegarder
        await _users.AjouterAsync(user, ct);
        await _users.SauvegarderAsync(ct);

        // 6️⃣ Retour sécurisé
        var response = new RegisterResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            Role = user.Role
        };

        return (true, 201, null, response);
    }
}
