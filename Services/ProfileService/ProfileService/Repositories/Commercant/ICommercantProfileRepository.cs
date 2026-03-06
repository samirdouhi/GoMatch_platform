using ProfileService.Models;

namespace ProfileService.Repositories.Commercant;

public interface ICommercantProfileRepository
{
    Task<CommercantProfile?> GetByUserIdAsync(Guid userId);
    Task<bool> ExistsByUserIdAsync(Guid userId);
    Task AddAsync(CommercantProfile profile);
    Task SaveChangesAsync();
}