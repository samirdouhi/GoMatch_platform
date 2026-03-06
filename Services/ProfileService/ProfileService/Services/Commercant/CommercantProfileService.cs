using ProfileService.DTOs.Commercant;
using ProfileService.DTOs.Common;
using ProfileService.ErrorHandling.Exceptions;
using ProfileService.Mappers.Commercant;
using ProfileService.Models;
using ProfileService.Repositories.Commercant;

namespace ProfileService.Services.Commercant;

public sealed class CommercantProfileService : ICommercantProfileService
{
    private readonly ICommercantProfileRepository _repository;
    private readonly ICommercantProfileMapper _mapper;

    public CommercantProfileService(
        ICommercantProfileRepository repository,
        ICommercantProfileMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<CommercantProfileResponseDto> GetByUserIdAsync(Guid userId, CancellationToken ct)
    {
        var profile = await _repository.GetByUserIdAsync(userId);
        if (profile is null)
            throw new NotFoundException("Le profil commerçant est introuvable.");

        return _mapper.ToResponseDto(profile);
    }

    public async Task<CommercantProfileResponseDto> InitProfileAsync(
        InitProfileRequestDto request,
        CancellationToken ct)
    {
        var exists = await _repository.ExistsByUserIdAsync(request.UserId);
        if (exists)
            throw new ConflictException("Le profil commerçant existe déjà.");

        var profile = new CommercantProfile
        {
            UserId = request.UserId,
            InscriptionTerminee = false
        };

        await _repository.AddAsync(profile);
        await _repository.SaveChangesAsync();

        return _mapper.ToResponseDto(profile);
    }

    public async Task<CommercantProfileResponseDto> CompleteProfileAsync(
        Guid userId,
        CommercantProfileRequestDto request,
        CancellationToken ct)
    {
        var profile = await _repository.GetByUserIdAsync(userId);
        if (profile is null)
            throw new NotFoundException("Le profil commerçant est introuvable.");

        _mapper.MapRequest(request, profile);
        profile.InscriptionTerminee = true;

        await _repository.SaveChangesAsync();

        return _mapper.ToResponseDto(profile);
    }

    public async Task<UpdateProfileResponseDto> UpdateProfileAsync(
        Guid userId,
        UpdateProfileRequestDto request,
        CancellationToken ct)
    {
        var profile = await _repository.GetByUserIdAsync(userId);
        if (profile is null)
            throw new NotFoundException("Le profil commerçant est introuvable.");

        _mapper.MapCommonUpdates(request, profile);

        await _repository.SaveChangesAsync();

        return _mapper.ToUpdateProfileResponseDto(profile);
    }
}