using System.Security.Claims;
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

    private static Guid? GetUserIdFromClaims(ClaimsPrincipal user)
    {
        var idValue =
            user.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
            user.FindFirst("sub")?.Value ??
            user.FindFirst("userId")?.Value;

        return Guid.TryParse(idValue, out var id) ? id : null;
    }

    public async Task<(bool Success, int Code, string? Erreur, RegisterResponseDto? Data)>
        RegisterAsync(RegisterRequestDto request, CancellationToken ct)
    {
        var email = LogSanitizer.NormalizeEmail(request.Email);
        _logger.LogInformation("REGISTER_ATTEMPT email={Email}", email);

        if (string.IsNullOrWhiteSpace(email))
        {
            _logger.LogWarning("REGISTER_VALIDATION_FAILED code=AUTH.EMAIL_REQUIRED");
            throw new ValidationException("Email obligatoire.", "AUTH.EMAIL_REQUIRED");
        }

        if (!_passwordPolicy.EstValide(request.Password, out var erreurPwd))
        {
            _logger.LogWarning("REGISTER_VALIDATION_FAILED email={Email} code=AUTH.PASSWORD_WEAK", email);
            throw new ValidationException(erreurPwd ?? "Mot de passe invalide.", "AUTH.PASSWORD_WEAK");
        }

        if (await _users.EmailExisteAsync(email, ct))
        {
            _logger.LogWarning("REGISTER_CONFLICT email={Email} code=AUTH.EMAIL_EXISTS", email);
            throw new ConflictException("Email déjà utilisé.", "AUTH.EMAIL_EXISTS");
        }

        var passwordHash = _passwordHasher.Hasher(request.Password);
        var user = _mapper.ToUser(email, passwordHash);

        user.Id = Guid.NewGuid();
        user.Role = UserRole.Touriste;
        user.IsActive = true;
        user.CreatedAt = DateTime.UtcNow;

        await _users.AjouterAsync(user, ct);
        await _users.SauvegarderAsync(ct);

        _logger.LogInformation(
            "REGISTER_SUCCESS userId={UserId} email={Email} role={Role}",
            user.Id, user.Email, user.Role);

        var response = _mapper.ToRegisterResponse(user);
        return (true, 201, null, response);
    }

    public async Task<(bool Success, int Code, string? Erreur, LoginResponseDto? Data)>
        LoginAsync(LoginRequestDto dto, CancellationToken ct)
    {
        var email = LogSanitizer.NormalizeEmail(dto.Email);
        _logger.LogInformation("LOGIN_ATTEMPT email={Email}", email);

        if (string.IsNullOrWhiteSpace(email))
        {
            _logger.LogWarning("LOGIN_VALIDATION_FAILED code=AUTH.EMAIL_REQUIRED");
            throw new ValidationException("Email obligatoire.", "AUTH.EMAIL_REQUIRED");
        }

        var motDePasse = dto.Password;
        if (string.IsNullOrWhiteSpace(motDePasse))
        {
            _logger.LogWarning("LOGIN_VALIDATION_FAILED email={Email} code=AUTH.PASSWORD_REQUIRED", email);
            throw new ValidationException("Mot de passe obligatoire.", "AUTH.PASSWORD_REQUIRED");
        }

        var user = await _users.GetByEmailAsync(email, ct);
        if (user is null)
        {
            _logger.LogWarning("LOGIN_FAILED email={Email} code=AUTH.INVALID_CREDENTIALS", email);
            throw new UnauthorizedException("Identifiants invalides.");
        }

        if (!user.IsActive)
        {
            _logger.LogWarning(
                "LOGIN_FORBIDDEN userId={UserId} email={Email} code=AUTH.INACTIVE_ACCOUNT",
                user.Id, user.Email);
            throw new ForbiddenException("Compte inactif.");
        }

        if (!_passwordHasher.Verifier(motDePasse, user.PasswordHash))
        {
            _logger.LogWarning(
                "LOGIN_FAILED userId={UserId} email={Email} code=AUTH.INVALID_CREDENTIALS",
                user.Id, user.Email);
            throw new UnauthorizedException("Identifiants invalides.");
        }

        var (accessToken, accessExpiresAtUtc) = _jwtService.GenerateToken(user);

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

        _logger.LogInformation(
            "LOGIN_SUCCESS userId={UserId} email={Email} role={Role} refreshTail={RefreshTail}",
            user.Id, user.Email, user.Role, LogSanitizer.TokenTail(refreshValue));

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
        var existing = await _refreshTokens.GetByTokenWithUserAsync(refreshToken, ct);

        if (existing is null)
        {
            _logger.LogWarning("REFRESH_FAILED tokenTail={Tail} reason=NOT_FOUND code=AUTH.UNAUTHORIZED", tail);
            throw new UnauthorizedException("Refresh token invalide.");
        }

        if (existing.ExpiresAtUtc <= now)
        {
            _logger.LogWarning(
                "REFRESH_FAILED userId={UserId} tokenTail={Tail} reason=EXPIRED code=AUTH.UNAUTHORIZED",
                existing.UserId, tail);
            throw new UnauthorizedException("Refresh token expiré.");
        }

        if (existing.RevokedAtUtc is not null)
        {
            _logger.LogWarning(
                "REFRESH_REVOKED userId={UserId} tokenTail={Tail} reason=REVOKED_REUSE_DETECTED",
                existing.UserId, tail);

            await _refreshTokens.RevokeAllActiveForUserAsync(existing.UserId, now, ct);
            await _refreshTokens.SaveChangesAsync(ct);

            throw new UnauthorizedException("Refresh token révoqué.");
        }

        var user = existing.User;
        if (user is null)
        {
            _logger.LogWarning(
                "REFRESH_FAILED userId={UserId} tokenTail={Tail} reason=USER_NULL",
                existing.UserId, tail);
            throw new UnauthorizedException("Utilisateur invalide.");
        }

        if (!user.IsActive)
        {
            _logger.LogWarning(
                "REFRESH_FORBIDDEN userId={UserId} email={Email} reason=USER_INACTIVE",
                user.Id, user.Email);
            throw new ForbiddenException("Utilisateur inactif.");
        }

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

        var (accessToken, accessExpiresAtUtc) = _jwtService.GenerateToken(user);

        await _refreshTokens.SaveChangesAsync(ct);

        _logger.LogInformation(
            "REFRESH_SUCCESS userId={UserId} email={Email} oldTail={OldTail} newTail={NewTail}",
            user.Id, user.Email, tail, newTail);

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

    public async Task<(bool Success, int Code, string? Erreur, AuthMeResponseDto? Data)>
        GetMeAsync(ClaimsPrincipal user, CancellationToken ct)
    {
        var userId = GetUserIdFromClaims(user);
        if (userId is null)
        {
            _logger.LogWarning("ME_FAILED code=AUTH.UNAUTHORIZED reason=CLAIM_MISSING");
            throw new UnauthorizedException("Utilisateur non authentifié.");
        }

        var entity = await _users.GetByIdAsync(userId.Value, ct);
        if (entity is null)
        {
            _logger.LogWarning("ME_FAILED userId={UserId} code=AUTH.USER_NOT_FOUND", userId.Value);
            throw new NotFoundException("Utilisateur introuvable.", "AUTH.USER_NOT_FOUND");
        }

        var response = new AuthMeResponseDto
        {
            UserId = entity.Id,
            Email = entity.Email,
            Role = entity.Role.ToString()
        };

        _logger.LogInformation("ME_SUCCESS userId={UserId} email={Email}", entity.Id, entity.Email);

        return (true, 200, null, response);
    }

    public async Task<(bool Success, int Code, string? Erreur)>
        ChangeEmailAsync(ClaimsPrincipal user, ChangeEmailRequestDto dto, CancellationToken ct)
    {
        var userId = GetUserIdFromClaims(user);
        if (userId is null)
        {
            _logger.LogWarning("CHANGE_EMAIL_FAILED code=AUTH.UNAUTHORIZED reason=CLAIM_MISSING");
            throw new UnauthorizedException("Utilisateur non authentifié.");
        }

        var entity = await _users.GetByIdAsync(userId.Value, ct);
        if (entity is null)
        {
            _logger.LogWarning("CHANGE_EMAIL_FAILED userId={UserId} code=AUTH.USER_NOT_FOUND", userId.Value);
            throw new NotFoundException("Utilisateur introuvable.", "AUTH.USER_NOT_FOUND");
        }

        if (!entity.IsActive)
        {
            _logger.LogWarning("CHANGE_EMAIL_FORBIDDEN userId={UserId} reason=INACTIVE", entity.Id);
            throw new ForbiddenException("Compte inactif.");
        }

        var currentPassword = dto.CurrentPassword?.Trim() ?? string.Empty;
        var newEmail = LogSanitizer.NormalizeEmail(dto.NewEmail);

        if (string.IsNullOrWhiteSpace(newEmail))
        {
            _logger.LogWarning("CHANGE_EMAIL_VALIDATION_FAILED userId={UserId} code=AUTH.EMAIL_REQUIRED", entity.Id);
            throw new ValidationException("Nouvel email obligatoire.", "AUTH.EMAIL_REQUIRED");
        }

        if (string.IsNullOrWhiteSpace(currentPassword))
        {
            _logger.LogWarning("CHANGE_EMAIL_VALIDATION_FAILED userId={UserId} code=AUTH.PASSWORD_REQUIRED", entity.Id);
            throw new ValidationException("Mot de passe actuel obligatoire.", "AUTH.PASSWORD_REQUIRED");
        }

        if (!_passwordHasher.Verifier(currentPassword, entity.PasswordHash))
        {
            _logger.LogWarning("CHANGE_EMAIL_FAILED userId={UserId} code=AUTH.INVALID_PASSWORD", entity.Id);
            throw new UnauthorizedException("Mot de passe actuel incorrect.");
        }

        if (string.Equals(entity.Email, newEmail, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("CHANGE_EMAIL_CONFLICT userId={UserId} code=AUTH.EMAIL_SAME", entity.Id);
            throw new ConflictException("Le nouvel email est identique à l’ancien.", "AUTH.EMAIL_SAME");
        }

        if (await _users.EmailExisteAsync(newEmail, ct))
        {
            _logger.LogWarning(
                "CHANGE_EMAIL_CONFLICT userId={UserId} newEmail={Email} code=AUTH.EMAIL_EXISTS",
                entity.Id, newEmail);
            throw new ConflictException("Email déjà utilisé.", "AUTH.EMAIL_EXISTS");
        }

        entity.Email = newEmail;
        await _users.SauvegarderAsync(ct);

        _logger.LogInformation("CHANGE_EMAIL_SUCCESS userId={UserId} newEmail={Email}", entity.Id, entity.Email);
        return (true, 200, null);
    }

    public async Task<(bool Success, int Code, string? Erreur)>
        ChangePasswordAsync(ClaimsPrincipal user, ChangePasswordRequestDto dto, CancellationToken ct)
    {
        var userId = GetUserIdFromClaims(user);
        if (userId is null)
        {
            _logger.LogWarning("CHANGE_PASSWORD_FAILED code=AUTH.UNAUTHORIZED reason=CLAIM_MISSING");
            throw new UnauthorizedException("Utilisateur non authentifié.");
        }

        var entity = await _users.GetByIdAsync(userId.Value, ct);
        if (entity is null)
        {
            _logger.LogWarning("CHANGE_PASSWORD_FAILED userId={UserId} code=AUTH.USER_NOT_FOUND", userId.Value);
            throw new NotFoundException("Utilisateur introuvable.", "AUTH.USER_NOT_FOUND");
        }

        if (!entity.IsActive)
        {
            _logger.LogWarning("CHANGE_PASSWORD_FORBIDDEN userId={UserId} reason=INACTIVE", entity.Id);
            throw new ForbiddenException("Compte inactif.");
        }

        var currentPassword = dto.CurrentPassword?.Trim() ?? string.Empty;
        var newPassword = dto.NewPassword ?? string.Empty;

        if (string.IsNullOrWhiteSpace(currentPassword))
        {
            _logger.LogWarning("CHANGE_PASSWORD_VALIDATION_FAILED userId={UserId} code=AUTH.PASSWORD_REQUIRED", entity.Id);
            throw new ValidationException("Mot de passe actuel obligatoire.", "AUTH.PASSWORD_REQUIRED");
        }

        if (string.IsNullOrWhiteSpace(newPassword))
        {
            _logger.LogWarning("CHANGE_PASSWORD_VALIDATION_FAILED userId={UserId} code=AUTH.NEW_PASSWORD_REQUIRED", entity.Id);
            throw new ValidationException("Nouveau mot de passe obligatoire.", "AUTH.NEW_PASSWORD_REQUIRED");
        }

        if (!_passwordPolicy.EstValide(newPassword, out var erreurPwd))
        {
            _logger.LogWarning("CHANGE_PASSWORD_VALIDATION_FAILED userId={UserId} code=AUTH.PASSWORD_WEAK", entity.Id);
            throw new ValidationException(erreurPwd ?? "Mot de passe invalide.", "AUTH.PASSWORD_WEAK");
        }

        if (!_passwordHasher.Verifier(currentPassword, entity.PasswordHash))
        {
            _logger.LogWarning("CHANGE_PASSWORD_FAILED userId={UserId} code=AUTH.INVALID_PASSWORD", entity.Id);
            throw new UnauthorizedException("Mot de passe actuel incorrect.");
        }

        if (_passwordHasher.Verifier(newPassword, entity.PasswordHash))
        {
            _logger.LogWarning("CHANGE_PASSWORD_CONFLICT userId={UserId} code=AUTH.PASSWORD_SAME", entity.Id);
            throw new ConflictException("Le nouveau mot de passe doit être différent de l’ancien.", "AUTH.PASSWORD_SAME");
        }

        entity.PasswordHash = _passwordHasher.Hasher(newPassword);
        await _users.SauvegarderAsync(ct);

        _logger.LogInformation("CHANGE_PASSWORD_SUCCESS userId={UserId}", entity.Id);
        return (true, 200, null);
    }
}