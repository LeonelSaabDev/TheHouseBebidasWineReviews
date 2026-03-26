using Microsoft.EntityFrameworkCore;
using TheHouseBebidas.WineReviews.Application.Interfaces.Persistence;
using TheHouseBebidas.WineReviews.Domain.Entities;

namespace TheHouseBebidas.WineReviews.Infrastructure.Persistence.Repositories;

public sealed class EfSiteContentRepository : ISiteContentRepository
{
    private readonly WineReviewsDbContext _dbContext;

    public EfSiteContentRepository(WineReviewsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<SiteContent>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SiteContents
            .AsNoTracking()
            .OrderBy(content => content.Key)
            .ToListAsync(cancellationToken);
    }

    public Task<SiteContent?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return _dbContext.SiteContents
            .FirstOrDefaultAsync(content => content.Key == key, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
