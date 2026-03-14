using ProfileService.Dtos.Touriste;
using ProfileService.DTOs.Common;
using ProfileService.DTOs.Touriste;
using ProfileService.Enums;
using ProfileService.ErrorHandling.Exceptions;
using ProfileService.Mappers.Touriste;
using ProfileService.Models;
using ProfileService.Repositories.Touriste;
using ProfileService.Security;

namespace ProfileService.Services.Touriste;

public sealed class TouristeProfileService : ITouristeProfileService
{
    private readonly ITouristeProfileRepository _repository;
    private readonly ITouristeProfileMapper _mapper;
    private readonly ICurrentUser _currentUser;

    public TouristeProfileService(
        ITouristeProfileRepository repository,
        ITouristeProfileMapper mapper,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<TouristeProfileResponseDto> GetMyProfileAsync(CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        var profile = await _repository.GetByUserIdAsync(userId);

        if (profile is null)
            throw new NotFoundException("Le profil touriste est introuvable.");

        return _mapper.ToProfileResponseDto(profile);
    }

    public async Task<TouristeProfileResponseDto> InitProfileAsync(CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        var exists = await _repository.ExistsByUserIdAsync(userId);

        if (exists)
            throw new ConflictException("Le profil touriste existe déjà.");

        var profile = new TouristeProfile
        {
            UserId = userId,
            InscriptionTerminee = false
        };

        await _repository.AddAsync(profile);
        await _repository.SaveChangesAsync();

        return _mapper.ToProfileResponseDto(profile);
    }

    public async Task<TouristeProfileResponseDto> RegisterInitAsync(
        RegisterInitProfileRequestDto request,
        CancellationToken ct)
    {
        var exists = await _repository.ExistsByUserIdAsync(request.UserId);

        if (exists)
            throw new ConflictException("Le profil touriste existe déjà.");

        Genre parsedGenre;
        var genreOk = Enum.TryParse<Genre>(request.Genre, true, out parsedGenre);

        if (!genreOk)
            throw new ConflictException("La valeur du genre est invalide.");

        var profile = new TouristeProfile
        {
            UserId = request.UserId,
            Prenom = request.Prenom,
            Nom = request.Nom,
            DateNaissance = DateOnly.FromDateTime(request.DateNaissance),
            Genre = parsedGenre,
            InscriptionTerminee = false
        };

        await _repository.AddAsync(profile);
        await _repository.SaveChangesAsync();

        return _mapper.ToProfileResponseDto(profile);
    }

    public async Task<UpdateProfileResponseDto> UpdateProfileAsync(
        UpdateProfileRequestDto request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        var profile = await _repository.GetByUserIdAsync(userId);

        if (profile is null)
            throw new NotFoundException("Le profil touriste est introuvable.");

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

    public async Task<OnboardingResponseDto> CompleteOnboardingAsync(
        OnboardingRequestDto request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        var profile = await _repository.GetByUserIdAsync(userId);

        if (profile is null)
            throw new NotFoundException("Le profil touriste est introuvable.");

        if (string.IsNullOrWhiteSpace(profile.Prenom) ||
            string.IsNullOrWhiteSpace(profile.Nom) ||
            profile.DateNaissance is null ||
            profile.Genre is null)
        {
            throw new ConflictException(
                "Le prénom, le nom, la date de naissance et le genre sont obligatoires avant de terminer l'inscription.");
        }

        _mapper.MapOnboarding(request, profile);
        profile.InscriptionTerminee = true;

        await _repository.SaveChangesAsync();

        return _mapper.ToOnboardingResponseDto(profile);
    }

    public async Task<PreferencesResponseDto> UpdatePreferencesAsync(
        UpdatePreferencesRequestDto request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        var profile = await _repository.GetByUserIdAsync(userId);

        if (profile is null)
            throw new NotFoundException("Le profil touriste est introuvable.");

        _mapper.MapPreferences(request, profile);

        await _repository.SaveChangesAsync();

        return _mapper.ToPreferencesResponseDto(profile);
    }
}