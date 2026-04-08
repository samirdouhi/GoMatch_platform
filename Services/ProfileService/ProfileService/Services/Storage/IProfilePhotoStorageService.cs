using Microsoft.AspNetCore.Http;

namespace ProfileService.Services.Storage;

public interface IProfilePhotoStorageService
{
    Task<string> SaveAsync(Guid userId, IFormFile photo, string? oldPhotoPath, CancellationToken ct);

    public Task<(Stream Stream, string ContentType)> OpenReadAsync(string photoPath, CancellationToken ct);
}
