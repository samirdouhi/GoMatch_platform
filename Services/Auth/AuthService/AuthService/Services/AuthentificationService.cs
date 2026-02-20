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
    private readonly IJwtService _jwtService;

    public AuthentificationService(
      IUserRepository users,
      IPasswordHasher passwordHasher,
      PasswordPolicy passwordPolicy,
      IJwtService jwtService)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _passwordPolicy = passwordPolicy;
        _jwtService = jwtService;
    }

    public async Task<(bool Success, int Code, string? Erreur, RegisterResponseDto? Data)>
        RegisterAsync(RegisterRequestDto request, CancellationToken ct)
    {
        // 1) Normaliser email
        var email = (request.Email ?? string.Empty)
            .Trim()
            .ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(email))
            return (false, 400, "Email obligatoire.", null);

        // 2) Vérifier mot de passe fort
        if (!_passwordPolicy.EstValide(request.Password, out var erreurPwd))
            return (false, 400, erreurPwd, null);

        // 3) Vérifier email déjà utilisé
        if (await _users.EmailExisteAsync(email, ct))
            return (false, 409, "Email déjà utilisé.", null);

        // 4) Créer utilisateur (Role forcé Touriste)
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = _passwordHasher.Hasher(request.Password),
            Role = UserRole.Touriste,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // 5) Sauvegarder
        await _users.AjouterAsync(user, ct);
        await _users.SauvegarderAsync(ct);

        // 6) Retour sécurisé
        var response = new RegisterResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            Role = user.Role
        };

        return (true, 201, null, response);
    }

    public async Task<(bool Success, int Code, string? Erreur, LoginResponseDto? Data)>
        LoginAsync(LoginRequestDto dto, CancellationToken ct)
    {
        // 1) Normaliser email
        var email = (dto.Email ?? string.Empty)
            .Trim()
            .ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(email))
            return (false, 400, "Email obligatoire.", null);

        // 2) Chercher utilisateur
        var user = await _users.GetByEmailAsync(email, ct);
        if (user is null)
            return (false, 401, "Identifiants invalides.", null);

        // 3) Vérifier compte actif
        if (!user.IsActive)
            return (false, 403, "Compte inactif.", null);

        // 4) Vérifier mot de passe
        // ⚠️ Adapte le nom selon ton DTO: MotDePasse ou Password
        var motDePasse = dto.Password; // si ton DTO a "Password", remplace ici par dto.Password

        if (!_passwordHasher.Verifier(motDePasse, user.PasswordHash))
            return (false, 401, "Identifiants invalides.", null);

        // 5) Générer JWT (étape suivante)
        // Pour l’instant on met un placeholder, juste pour valider le flux.

        var (token, expires) = _jwtService.GenerateToken(user);

        var response = new LoginResponseDto
        {
            AccessToken = token,
            ExpiresAtUtc = expires
        };

        return (true, 200, null, response);


    }
}
