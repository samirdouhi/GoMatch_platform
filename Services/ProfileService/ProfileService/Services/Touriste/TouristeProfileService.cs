using ProfileService.DTOs.Common;
using ProfileService.DTOs.Touriste;
using ProfileService.ErrorHandling.Exceptions;
using ProfileService.Mappers.Touriste;
using ProfileService.Models;
using ProfileService.Repositories.Touriste;

namespace ProfileService.Services.Touriste;

public sealed class TouristeProfileService : ITouristeProfileService
{
    private readonly ITouristeProfileRepository _repository;
    private readonly ITouristeProfileMapper _mapper;

    public TouristeProfileService(
        ITouristeProfileRepository repository,
        ITouristeProfileMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<TouristeProfileResponseDto> GetByUserIdAsync(Guid userId, CancellationToken ct)
    {
        var profile = await _repository.GetByUserIdAsync(userId);
        if (profile is null)
            throw new NotFoundException("Le profil touriste est introuvable.");

        return _mapper.ToProfileResponseDto(profile);
    }

    public async Task<TouristeProfileResponseDto> InitProfileAsync(
        InitProfileRequestDto request,
        CancellationToken ct)
    {
        var exists = await _repository.ExistsByUserIdAsync(request.UserId);
        if (exists)
            throw new ConflictException("Le profil touriste existe déjà.");

        var profile = new TouristeProfile
        {
            UserId = request.UserId,
            InscriptionTerminee = false
        };

        await _repository.AddAsync(profile);
        await _repository.SaveChangesAsync();

        return _mapper.ToProfileResponseDto(profile);
    }

    public async Task<UpdateProfileResponseDto> UpdateProfileAsync(
        Guid userId,
        UpdateProfileRequestDto request,
        CancellationToken ct)
    {
        var profile = await _repository.GetByUserIdAsync(userId);
        if (profile is null)
            throw new NotFoundException("Le profil touriste est introuvable.");

        _mapper.MapCommonUpdates(request, profile);

        await _repository.SaveChangesAsync();

        return _mapper.ToUpdateProfileResponseDto(profile);
    }

    public async Task<OnboardingResponseDto> CompleteOnboardingAsync(
        Guid userId,
        OnboardingRequestDto request,
        CancellationToken ct)
    {
        var profile = await _repository.GetByUserIdAsync(userId);
        if (profile is null)
            throw new NotFoundException("Le profil touriste est introuvable.");

        _mapper.MapOnboarding(request, profile);
        profile.InscriptionTerminee = true;

        await _repository.SaveChangesAsync();

        return _mapper.ToOnboardingResponseDto(profile);
    }

    public async Task<PreferencesResponseDto> UpdatePreferencesAsync(
        Guid userId,
        UpdatePreferencesRequestDto request,
        CancellationToken ct)
    {
        var profile = await _repository.GetByUserIdAsync(userId);
        if (profile is null)
            throw new NotFoundException("Le profil touriste est introuvable.");

        _mapper.MapPreferences(request, profile);

        await _repository.SaveChangesAsync();

        return _mapper.ToPreferencesResponseDto(profile);
    }
}