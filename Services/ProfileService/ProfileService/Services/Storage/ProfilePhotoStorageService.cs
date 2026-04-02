using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using ProfileService.Options;

namespace ProfileService.Services.Storage;

public sealed class ProfilePhotoStorageService : IProfilePhotoStorageService
{
    private readonly PhotoStorageOptions _options;
    private readonly IWebHostEnvironment _env;

    public ProfilePhotoStorageService(
        IOptions<PhotoStorageOptions> options,
        IWebHostEnvironment env)
    {
        _options = options.Value;
        _env = env;
    }

    public async Task<string> SaveAsync(Guid userId, IFormFile photo, string? oldPhotoPath, CancellationToken ct)
    {
        var rootPath = Path.Combine(_env.ContentRootPath, _options.RootPath);
        Directory.CreateDirectory(rootPath);

        var fileName = $"{userId}_{Guid.NewGuid():N}{Path.GetExtension(photo.FileName)}";
        var filePath = Path.Combine(rootPath, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await photo.CopyToAsync(stream, ct);

        return fileName;
    }

    public async Task<(Stream Stream, string ContentType)> OpenReadAsync(string photoPath, CancellationToken ct)
    {
        var rootPath = Path.Combine(_env.ContentRootPath, _options.RootPath);
        var fullPath = Path.Combine(rootPath, photoPath);

        var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        return (stream, "image/jpeg");
    }
}
