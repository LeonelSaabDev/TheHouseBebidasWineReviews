using TheHouseBebidas.WineReviews.Domain.Entities;

namespace TheHouseBebidas.WineReviews.Application.Interfaces.Persistence;

public interface ISiteContentRepository
{
    Task<IReadOnlyCollection<SiteContent>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<SiteContent?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
