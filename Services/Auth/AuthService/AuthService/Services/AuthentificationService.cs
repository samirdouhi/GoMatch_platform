using AuthService.DTOs;
using AuthService.Enums;
using AuthService.Exceptions;
using AuthService.Logging;
using AuthService.Mappers;
using AuthService.Models;
using AuthService.Repositories;
using AuthService.Security;
using Microsoft.Extensions.Logging;

namespace AuthService.Services;

public sealed class AuthentificationService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IPasswordHasher _passwordHasher;
    private readonly PasswordPolicy _passwordPolicy;
    private readonly IJwtService _jwtService;
    private readonly IAuthMapper _mapper;
    private readonly ILogger<AuthentificationService> _logger;

    private static readonly TimeSpan RefreshLifetime = TimeSpan.FromDays(7);

    public AuthentificationService(
        IUserRepository users,
        IRefreshTokenRepository refreshTokens,
        IPasswordHasher passwordHasher,
        PasswordPolicy passwordPolicy,
        IJwtService jwtService,
        IAuthMapper mapper,
        ILogger<AuthentificationService> logger)
    {
        _users = users;
        _refreshTokens = refreshTokens;
        _passwordHasher = passwordHasher;
        _passwordPolicy = passwordPolicy;
        _jwtService = jwtService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<(bool Success, int Code, string? Erreur, RegisterResponseDto? Data)>
        RegisterAsync(RegisterRequestDto request, CancellationToken ct)
    {
        // 1) Normaliser email
        var email = LogSanitizer.NormalizeEmail(request.Email);
        _logger.LogInformation("REGISTER_ATTEMPT email={Email}", email);

        if (string.IsNullOrWhiteSpace(email))
        {
            _logger.LogWarning("REGISTER_VALIDATION_FAILED code=AUTH.EMAIL_REQUIRED");
            throw new ValidationException("Email obligatoire.", "AUTH.EMAIL_REQUIRED");
        }

        // 2) Vérifier mot de passe fort (NE JAMAIS logger le mot de passe)
        if (!_passwordPolicy.EstValide(request.Password, out var erreurPwd))
        {
            _logger.LogWarning("REGISTER_VALIDATION_FAILED email={Email} code=AUTH.PASSWORD_WEAK", email);
            throw new ValidationException(erreurPwd, "AUTH.PASSWORD_WEAK");
        }

        // 3) Vérifier email déjà utilisé
        if (await _users.EmailExisteAsync(email, ct))
        {
            _logger.LogWarning("REGISTER_CONFLICT email={Email} code=AUTH.EMAIL_EXISTS", email);
            throw new ConflictException("Email déjà utilisé.", "AUTH.EMAIL_EXISTS");
        }

        // 4) Hasher mot de passe
        var passwordHash = _passwordHasher.Hasher(request.Password);

        // 5) Mapper -> entité (SRP: mapping hors service)
        var user = _mapper.ToUser(email, passwordHash);

        // 6) Logique métier (valeurs serveur)
        user.Id = Guid.NewGuid();
        user.Role = UserRole.Touriste;   // rôle par défaut côté serveur
        user.IsActive = true;
        user.CreatedAt = DateTime.UtcNow;

        // 7) Sauvegarder
        await _users.AjouterAsync(user, ct);
        await _users.SauvegarderAsync(ct);

        // 8) Log succès
        _logger.LogInformation("REGISTER_SUCCESS userId={UserId} email={Email} role={Role}",
            user.Id, user.Email, user.Role);

        // 9) Retour success
        var response = _mapper.ToRegisterResponse(user);
        return (true, 201, null, response);
    }

    public async Task<(bool Success, int Code, string? Erreur, LoginResponseDto? Data)>
        LoginAsync(LoginRequestDto dto, CancellationToken ct)
    {
        // 1) Normaliser email
        var email = LogSanitizer.NormalizeEmail(dto.Email);
        _logger.LogInformation("LOGIN_ATTEMPT email={Email}", email);

        if (string.IsNullOrWhiteSpace(email))
        {
            _logger.LogWarning("LOGIN_VALIDATION_FAILED code=AUTH.EMAIL_REQUIRED");
            throw new ValidationException("Email obligatoire.", "AUTH.EMAIL_REQUIRED");
        }

        // 2) Mot de passe obligatoire (NE JAMAIS logger le mot de passe)
        var motDePasse = dto.Password;
        if (string.IsNullOrWhiteSpace(motDePasse))
        {
            _logger.LogWarning("LOGIN_VALIDATION_FAILED email={Email} code=AUTH.PASSWORD_REQUIRED", email);
            throw new ValidationException("Mot de passe obligatoire.", "AUTH.PASSWORD_REQUIRED");
        }

        // 3) Chercher utilisateur
        var user = await _users.GetByEmailAsync(email, ct);
        if (user is null)
        {
            _logger.LogWarning("LOGIN_FAILED email={Email} code=AUTH.INVALID_CREDENTIALS", email);
            throw new UnauthorizedException("Identifiants invalides.");
        }

        // 4) Vérifier compte actif
        if (!user.IsActive)
        {
            _logger.LogWarning("LOGIN_FORBIDDEN userId={UserId} email={Email} code=AUTH.INACTIVE_ACCOUNT",
                user.Id, user.Email);
            throw new ForbiddenException("Compte inactif.");
        }

        // 5) Vérifier mot de passe
        if (!_passwordHasher.Verifier(motDePasse, user.PasswordHash))
        {
            _logger.LogWarning("LOGIN_FAILED userId={UserId} email={Email} code=AUTH.INVALID_CREDENTIALS",
                user.Id, user.Email);
            throw new UnauthorizedException("Identifiants invalides.");
        }

        // 6) Générer JWT (NE JAMAIS logger le token complet)
        var (accessToken, accessExpiresAtUtc) = _jwtService.GenerateToken(user);

        // 7) Générer + sauver RefreshToken (7 jours)
        var now = DateTime.UtcNow;
        var refreshValue = RefreshTokenGenerator.GenerateOpaqueToken();

        var refresh = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshValue,
            UserId = user.Id,
            CreatedAtUtc = now,
            ExpiresAtUtc = now.Add(RefreshLifetime),
            RevokedAtUtc = null,
            ReplacedByToken = null
        };

        await _refreshTokens.AddAsync(refresh, ct);
        await _refreshTokens.SaveChangesAsync(ct);

        _logger.LogInformation("LOGIN_SUCCESS userId={UserId} email={Email} role={Role} refreshTail={RefreshTail}",
            user.Id, user.Email, user.Role, LogSanitizer.TokenTail(refreshValue));

        // 8) Mapper -> réponse
        var response = _mapper.ToLoginResponse(accessToken, accessExpiresAtUtc, refreshValue);
        return (true, 200, null, response);
    }

    public async Task<(bool Success, int Code, string? Erreur, RefreshResponseDto? Data)>
        RefreshAsync(RefreshRequestDto dto, CancellationToken ct)
    {
        var refreshToken = (dto.RefreshToken ?? string.Empty).Trim();
        var tail = LogSanitizer.TokenTail(refreshToken);

        _logger.LogInformation("REFRESH_ATTEMPT tokenTail={Tail}", tail);

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            _logger.LogWarning("REFRESH_VALIDATION_FAILED code=AUTH.REFRESH_REQUIRED");
            throw new ValidationException("Refresh token obligatoire.", "AUTH.REFRESH_REQUIRED");
        }

        var now = DateTime.UtcNow;

        // 1) Charger token + user
        var existing = await _refreshTokens.GetByTokenWithUserAsync(refreshToken, ct);

        // 401: inexistant
        if (existing is null)
        {
            _logger.LogWarning("REFRESH_FAILED tokenTail={Tail} reason=NOT_FOUND code=AUTH.UNAUTHORIZED", tail);
            throw new UnauthorizedException("Refresh token invalide.");
        }

        // 401: expiré
        if (existing.ExpiresAtUtc <= now)
        {
            _logger.LogWarning("REFRESH_FAILED userId={UserId} tokenTail={Tail} reason=EXPIRED code=AUTH.UNAUTHORIZED",
                existing.UserId, tail);
            throw new UnauthorizedException("Refresh token expiré.");
        }

        // 401: révoqué (reuse detection => on révoque toute la session)
        if (existing.RevokedAtUtc is not null)
        {
            _logger.LogWarning("REFRESH_REVOKED userId={UserId} tokenTail={Tail} reason=REVOKED_REUSE_DETECTED",
                existing.UserId, tail);

            await _refreshTokens.RevokeAllActiveForUserAsync(existing.UserId, now, ct);
            await _refreshTokens.SaveChangesAsync(ct);

            throw new UnauthorizedException("Refresh token révoqué.");
        }

        // 401/403: user invalide/inactif
        var user = existing.User;
        if (user is null)
        {
            _logger.LogWarning("REFRESH_FAILED userId={UserId} tokenTail={Tail} reason=USER_NULL",
                existing.UserId, tail);
            throw new UnauthorizedException("Utilisateur invalide.");
        }

        if (!user.IsActive)
        {
            _logger.LogWarning("REFRESH_FORBIDDEN userId={UserId} email={Email} reason=USER_INACTIVE",
                user.Id, user.Email);
            throw new ForbiddenException("Utilisateur inactif.");
        }

        // 2) Rotation: révoquer l’ancien + créer nouveau
        var newRefreshValue = RefreshTokenGenerator.GenerateOpaqueToken();
        var newTail = LogSanitizer.TokenTail(newRefreshValue);

        existing.RevokedAtUtc = now;
        existing.ReplacedByToken = newRefreshValue;

        var newRefresh = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = newRefreshValue,
            UserId = user.Id,
            CreatedAtUtc = now,
            ExpiresAtUtc = now.Add(RefreshLifetime),
            RevokedAtUtc = null,
            ReplacedByToken = null
        };

        await _refreshTokens.AddAsync(newRefresh, ct);

        // 3) Nouveau JWT
        var (accessToken, accessExpiresAtUtc) = _jwtService.GenerateToken(user);

        // 4) Commit
        await _refreshTokens.SaveChangesAsync(ct);

        _logger.LogInformation("REFRESH_SUCCESS userId={UserId} email={Email} oldTail={OldTail} newTail={NewTail}",
            user.Id, user.Email, tail, newTail);

        // 5) Réponse
        var response = _mapper.ToRefreshResponse(accessToken, accessExpiresAtUtc, newRefreshValue);
        return (true, 200, null, response);
    }

    public async Task<(bool Success, int Code, string? Erreur)>
        LogoutAsync(string refreshToken, CancellationToken ct)
    {
        var token = (refreshToken ?? string.Empty).Trim();
        var tail = LogSanitizer.TokenTail(token);

        _logger.LogInformation("LOGOUT_ATTEMPT tokenTail={Tail}", tail);

        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogWarning("LOGOUT_VALIDATION_FAILED code=AUTH.REFRESH_REQUIRED");
            throw new ValidationException("Refresh token obligatoire.", "AUTH.REFRESH_REQUIRED");
        }

        var existing = await _refreshTokens.GetByTokenWithUserAsync(token, ct);

        if (existing is null)
        {
            _logger.LogWarning("LOGOUT_FAILED tokenTail={Tail} reason=NOT_FOUND code=AUTH.UNAUTHORIZED", tail);
            throw new UnauthorizedException("Refresh token invalide.");
        }

        // déjà logout => idempotent
        if (existing.RevokedAtUtc is not null)
        {
            _logger.LogInformation("LOGOUT_ALREADY_DONE userId={UserId} tokenTail={Tail}", existing.UserId, tail);
            return (true, 200, null);
        }

        existing.RevokedAtUtc = DateTime.UtcNow;
        await _refreshTokens.SaveChangesAsync(ct);

        _logger.LogInformation("LOGOUT_SUCCESS userId={UserId} tokenTail={Tail}", existing.UserId, tail);
        return (true, 200, null);
    }
}
