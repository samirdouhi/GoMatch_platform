using Microsoft.EntityFrameworkCore;
using ProfileService.Data;
using ProfileService.Models;

namespace ProfileService.Repositories.Touriste;

public sealed class TouristeProfileRepository : ITouristeProfileRepository
{
    private readonly ProfileDbContext _context;

    public TouristeProfileRepository(ProfileDbContext context)
    {
        _context = context;
    }

    public async Task<TouristeProfile?> GetByUserIdAsync(Guid userId)
    {
        return await _context.Profiles
            .OfType<TouristeProfile>()
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<bool> ExistsByUserIdAsync(Guid userId)
    {
        return await _context.Profiles
            .OfType<TouristeProfile>()
            .AnyAsync(p => p.UserId == userId);
    }

    public async Task AddAsync(TouristeProfile profile)
    {
        await _context.Profiles.AddAsync(profile);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}