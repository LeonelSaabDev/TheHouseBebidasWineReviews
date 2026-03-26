using TheHouseBebidas.WineReviews.Application.Interfaces.Persistence.Models;
using TheHouseBebidas.WineReviews.Domain.Entities;

namespace TheHouseBebidas.WineReviews.Application.Interfaces.Persistence;

public interface IReviewRepository
{
    Task<IReadOnlyCollection<Review>> GetVisibleByWineIdAsync(Guid wineId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Review>> GetAdminReviewsAsync(AdminReviewListQuery query, CancellationToken cancellationToken = default);

    Task AddAsync(Review review, CancellationToken cancellationToken = default);

    Task<bool> DeleteByIdAsync(Guid reviewId, CancellationToken cancellationToken = default);

    Task<WineReviewMetrics> GetVisibleMetricsByWineIdAsync(Guid wineId, CancellationToken cancellationToken = default);
}
