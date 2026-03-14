using ProfileService.DTOs.Admin;
using ProfileService.DTOs.Common;
using ProfileService.ErrorHandling.Exceptions;
using ProfileService.Mappers.Admin;
using ProfileService.Models;
using ProfileService.Repositories.Admin;
using ProfileService.Security;

namespace ProfileService.Services.Admin;

public sealed class AdminProfileService : IAdminProfileService
{
    private readonly IAdminProfileRepository _repository;
    private readonly IAdminProfileMapper _mapper;
    private readonly ICurrentUser _currentUser;

    public AdminProfileService(
        IAdminProfileRepository repository,
        IAdminProfileMapper mapper,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<AdminProfileResponseDto> GetMyProfileAsync(CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        var profile = await _repository.GetByUserIdAsync(userId);

        if (profile is null)
            throw new NotFoundException("Le profil admin est introuvable.");

        return _mapper.ToResponseDto(profile);
    }

    public async Task<AdminProfileResponseDto> InitProfileAsync(CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        var exists = await _repository.ExistsByUserIdAsync(userId);

        if (exists)
            throw new ConflictException("Le profil admin existe déjà.");

        var profile = new AdminProfile
        {
            UserId = userId,
            InscriptionTerminee = false
        };

        await _repository.AddAsync(profile);
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
            throw new NotFoundException("Le profil admin est introuvable.");

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