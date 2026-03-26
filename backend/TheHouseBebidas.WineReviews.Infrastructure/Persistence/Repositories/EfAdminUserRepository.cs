using Microsoft.EntityFrameworkCore;
using TheHouseBebidas.WineReviews.Application.Interfaces.Persistence;
using TheHouseBebidas.WineReviews.Domain.Entities;

namespace TheHouseBebidas.WineReviews.Infrastructure.Persistence.Repositories;

public sealed class EfAdminUserRepository : IAdminUserRepository
{
    private readonly WineReviewsDbContext _dbContext;

    public EfAdminUserRepository(WineReviewsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AdminUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return null;
        }

        var normalizedUsername = username.Trim();

        return await _dbContext.AdminUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Username == normalizedUsername, cancellationToken);
    }
}
