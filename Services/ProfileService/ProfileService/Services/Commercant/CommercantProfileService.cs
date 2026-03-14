using ProfileService.DTOs.Commercant;
using ProfileService.DTOs.Common;
using ProfileService.ErrorHandling.Exceptions;
using ProfileService.Mappers.Commercant;
using ProfileService.Models;
using ProfileService.Repositories.Commercant;
using ProfileService.Security;

namespace ProfileService.Services.Commercant;

public sealed class CommercantProfileService : ICommercantProfileService
{
    private readonly ICommercantProfileRepository _repository;
    private readonly ICommercantProfileMapper _mapper;
    private readonly ICurrentUser _currentUser;

    public CommercantProfileService(
        ICommercantProfileRepository repository,
        ICommercantProfileMapper mapper,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<CommercantProfileResponseDto> GetMyProfileAsync(CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        var profile = await _repository.GetByUserIdAsync(userId);
        if (profile is null)
            throw new NotFoundException("Le profil commerçant est introuvable.");

        return _mapper.ToResponseDto(profile);
    }

    public async Task<CommercantProfileResponseDto> InitProfileAsync(CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        var exists = await _repository.ExistsByUserIdAsync(userId);
        if (exists)
            throw new ConflictException("Le profil commerçant existe déjà.");

        var profile = new CommercantProfile
        {
            UserId = userId,
            InscriptionTerminee = false
        };

        await _repository.AddAsync(profile);
        await _repository.SaveChangesAsync();

        return _mapper.ToResponseDto(profile);
    }

    public async Task<CommercantProfileResponseDto> CompleteProfileAsync(
        CommercantProfileRequestDto request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        var profile = await _repository.GetByUserIdAsync(userId);
        if (profile is null)
            throw new NotFoundException("Le profil commerçant est introuvable.");

        _mapper.MapRequest(request, profile);

        if (string.IsNullOrWhiteSpace(profile.Prenom) ||
            string.IsNullOrWhiteSpace(profile.Nom) ||
            profile.DateNaissance is null ||
            profile.Genre is null ||
            string.IsNullOrWhiteSpace(profile.Telephone))
        {
            throw new ConflictException(
                "Le prénom, le nom, la date de naissance, le genre et le téléphone sont obligatoires avant de terminer l'inscription.");
        }

        profile.InscriptionTerminee = true;

        await _repository.SaveChangesAsync();

        return _mapper.ToResponseDto(profile);
    }

    public async Task<UpdateProfileResponseDto> UpdateProfileAsync(
        UpdateProfileRequestDto request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        var profile = await _repository.GetByUserIdAsync(userId);
        if (profile is null)
            throw new NotFoundException("Le profil commerçant est introuvable.");

        if (string.IsNullOrWhiteSpace(request.Prenom))
            throw new ConflictException("Le prénom est obligatoire.");

        if (string.IsNullOrWhiteSpace(request.Nom))
            throw new ConflictException("Le nom est obligatoire.");

        if (request.DateNaissance is null)
            throw new ConflictException("La date de naissance est obligatoire.");

        if (request.Genre is null)
            throw new ConflictException("Le genre est obligatoire.");

        _mapper.MapCommonUpdates(request, profile);

        await _repository.SaveChangesAsync();

        return _mapper.ToUpdateProfileResponseDto(profile);
    }
}