using Microsoft.EntityFrameworkCore;
using ProfileService.Data;
using ProfileService.Models;

namespace ProfileService.Repositories.Commercant;

public sealed class CommercantProfileRepository : ICommercantProfileRepository
{
    private readonly ProfileDbContext _context;

    public CommercantProfileRepository(ProfileDbContext context)
    {
        _context = context;
    }

    public async Task<CommercantProfile?> GetByUserIdAsync(Guid userId)
    {
        return await _context.Profiles
            .OfType<CommercantProfile>()
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<bool> ExistsByUserIdAsync(Guid userId)
    {
        return await _context.Profiles
            .OfType<CommercantProfile>()
            .AnyAsync(p => p.UserId == userId);
    }

    public async Task AddAsync(CommercantProfile profile)
    {
        await _context.Profiles.AddAsync(profile);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}