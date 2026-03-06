using ProfileService.DTOs.Admin;
using ProfileService.DTOs.Common;
using ProfileService.ErrorHandling.Exceptions;
using ProfileService.Mappers.Admin;
using ProfileService.Models;
using ProfileService.Repositories.Admin;

namespace ProfileService.Services.Admin;

public sealed class AdminProfileService : IAdminProfileService
{
    private readonly IAdminProfileRepository _repository;
    private readonly IAdminProfileMapper _mapper;

    public AdminProfileService(
        IAdminProfileRepository repository,
        IAdminProfileMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<AdminProfileResponseDto> GetByUserIdAsync(Guid userId, CancellationToken ct)
    {
        var profile = await _repository.GetByUserIdAsync(userId);

        if (profile is null)
            throw new NotFoundException("Le profil admin est introuvable.");

        return _mapper.ToResponseDto(profile);
    }

    public async Task<AdminProfileResponseDto> InitProfileAsync(
        InitProfileRequestDto request,
        CancellationToken ct)
    {
        var exists = await _repository.ExistsByUserIdAsync(request.UserId);

        if (exists)
            throw new ConflictException("Le profil admin existe déjà.");

        var profile = new AdminProfile
        {
            UserId = request.UserId,
            InscriptionTerminee = false
        };

        await _repository.AddAsync(profile);
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
            throw new NotFoundException("Le profil admin est introuvable.");

        _mapper.MapCommonUpdates(request, profile);

        await _repository.SaveChangesAsync();

        return _mapper.ToUpdateProfileResponseDto(profile);
    }
}