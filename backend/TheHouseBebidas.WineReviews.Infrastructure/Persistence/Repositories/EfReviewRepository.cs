using Microsoft.EntityFrameworkCore;
using TheHouseBebidas.WineReviews.Application.Interfaces.Persistence;
using TheHouseBebidas.WineReviews.Application.Interfaces.Persistence.Models;
using TheHouseBebidas.WineReviews.Domain.Entities;

namespace TheHouseBebidas.WineReviews.Infrastructure.Persistence.Repositories;

public sealed class EfReviewRepository : IReviewRepository
{
    private readonly WineReviewsDbContext _dbContext;

    public EfReviewRepository(WineReviewsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<Review>> GetVisibleByWineIdAsync(Guid wineId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Reviews
            .AsNoTracking()
            .Where(review => review.WineId == wineId && review.IsVisible)
            .OrderByDescending(review => review.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Review>> GetAdminReviewsAsync(AdminReviewListQuery query, CancellationToken cancellationToken = default)
    {
        var queryable = _dbContext.Reviews
            .AsNoTracking();

        if (query.WineId.HasValue)
        {
            queryable = queryable.Where(review => review.WineId == query.WineId.Value);
        }

        if (query.IsVisible.HasValue)
        {
            queryable = queryable.Where(review => review.IsVisible == query.IsVisible.Value);
        }

        if (query.Rating.HasValue)
        {
            queryable = queryable.Where(review => review.Rating == query.Rating.Value);
        }

        if (query.CreatedFrom.HasValue)
        {
            queryable = queryable.Where(review => review.CreatedAt >= query.CreatedFrom.Value);
        }

        if (query.CreatedTo.HasValue)
        {
            queryable = queryable.Where(review => review.CreatedAt <= query.CreatedTo.Value);
        }

        return await queryable
            .OrderByDescending(review => review.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Review review, CancellationToken cancellationToken = default)
    {
        await _dbContext.Reviews.AddAsync(review, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteByIdAsync(Guid reviewId, CancellationToken cancellationToken = default)
    {
        var review = await _dbContext.Reviews
            .FirstOrDefaultAsync(item => item.Id == reviewId, cancellationToken);

        if (review is null)
        {
            return false;
        }

        _dbContext.Reviews.Remove(review);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<WineReviewMetrics> GetVisibleMetricsByWineIdAsync(Guid wineId, CancellationToken cancellationToken = default)
    {
        var metrics = await _dbContext.Reviews
            .AsNoTracking()
            .Where(review => review.WineId == wineId && review.IsVisible)
            .GroupBy(static _ => 1)
            .Select(group => new
            {
                AverageRating = group.Select(review => (decimal?)review.Rating).Average() ?? 0m,
                ReviewsCount = group.Count()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return metrics is null
            ? new WineReviewMetrics(0m, 0)
            : new WineReviewMetrics(Math.Round(metrics.AverageRating, 2), metrics.ReviewsCount);
    }
}
