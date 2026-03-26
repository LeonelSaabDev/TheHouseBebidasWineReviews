using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TheHouseBebidas.WineReviews.Application.Interfaces.Persistence;
using TheHouseBebidas.WineReviews.Application.Interfaces.Persistence.Models;
using TheHouseBebidas.WineReviews.Domain.Entities;

namespace TheHouseBebidas.WineReviews.Infrastructure.Persistence.Repositories;

public sealed class EfWineRepository : IWineRepository
{
    private sealed class WineWithMetrics
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Winery { get; init; } = string.Empty;
        public int Year { get; init; }
        public string GrapeVariety { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string ImageUrl { get; init; } = string.Empty;
        public string? SecondaryImageUrl { get; init; }
        public string? FeaturedReviewSummary { get; init; }
        public bool IsActive { get; init; }
        public decimal AverageRating { get; init; }
        public int ReviewsCount { get; init; }
    }

    private readonly WineReviewsDbContext _dbContext;
    private static readonly Expression<Func<WineWithMetrics, WineDetailProjection>> WineDetailProjectionSelector =
        wine => new WineDetailProjection(
            wine.Id,
            wine.Name,
            wine.Winery,
            wine.Year,
            wine.GrapeVariety,
            wine.Description,
            wine.ImageUrl,
            wine.SecondaryImageUrl,
            Math.Round(wine.AverageRating, 2),
            wine.ReviewsCount,
            wine.FeaturedReviewSummary,
            wine.IsActive);

    private static readonly Expression<Func<WineWithMetrics, WineListProjection>> WineListProjectionSelector =
        item => new WineListProjection(
            item.Id,
            item.Name,
            item.Winery,
            item.Year,
            item.GrapeVariety,
            item.ImageUrl,
            item.SecondaryImageUrl,
            Math.Round(item.AverageRating, 2),
            item.ReviewsCount,
            item.FeaturedReviewSummary);

    public EfWineRepository(WineReviewsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<WineListProjection>> GetPublicWinesAsync(WineListQuery query, CancellationToken cancellationToken = default)
    {
        var normalizedSearchTerm = query.SearchTerm?.Trim();
        var queryable = BuildWinesWithMetricsQuery(onlyActive: true);

        if (!string.IsNullOrWhiteSpace(normalizedSearchTerm))
        {
            var escapedPattern = EscapeLikePattern(normalizedSearchTerm);
            var searchPattern = $"%{escapedPattern}%";

            queryable = queryable.Where(item =>
                EF.Functions.Like(item.Name, searchPattern, "\\") ||
                EF.Functions.Like(item.Winery, searchPattern, "\\") ||
                EF.Functions.Like(item.GrapeVariety, searchPattern, "\\"));
        }

        if (query.MinimumRating.HasValue)
        {
            queryable = queryable.Where(item => item.AverageRating >= query.MinimumRating.Value);
        }

        if (query.MaximumRating.HasValue)
        {
            queryable = queryable.Where(item => item.AverageRating <= query.MaximumRating.Value);
        }

        queryable = ApplyOrdering(queryable, query.SortBy, query.SortDescending);

        var totalItems = await queryable.CountAsync(cancellationToken);
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)query.PageSize));

        var items = await queryable
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(WineListProjectionSelector)
            .ToListAsync(cancellationToken);

        return new PagedResult<WineListProjection>(items, totalItems, totalPages, query.Page, query.PageSize);
    }

    public async Task<WineDetailProjection?> GetPublicWineDetailByIdAsync(Guid wineId, CancellationToken cancellationToken = default)
    {
        return await BuildWinesWithMetricsQuery(onlyActive: true)
            .Where(wine => wine.Id == wineId)
            .Select(WineDetailProjectionSelector)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<WineDetailProjection?> GetWineDetailByIdAsync(Guid wineId, CancellationToken cancellationToken = default)
    {
        return await BuildWinesWithMetricsQuery(onlyActive: false)
            .Where(wine => wine.Id == wineId)
            .Select(WineDetailProjectionSelector)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<bool> ExistsActiveWineAsync(Guid wineId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Wines
            .AsNoTracking()
            .AnyAsync(wine => wine.Id == wineId && wine.IsActive, cancellationToken);
    }

    public Task<Wine?> GetByIdAsync(Guid wineId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Wines
            .FirstOrDefaultAsync(wine => wine.Id == wineId, cancellationToken);
    }

    public async Task AddAsync(Wine wine, CancellationToken cancellationToken = default)
    {
        await _dbContext.Wines.AddAsync(wine, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<WineWithMetrics> BuildWinesWithMetricsQuery(bool onlyActive)
    {
        return from wine in _dbContext.Wines.AsNoTracking().Where(wine => !onlyActive || wine.IsActive)
               join review in _dbContext.Reviews.AsNoTracking().Where(review => review.IsVisible)
                   on wine.Id equals review.WineId into visibleReviews
               select new WineWithMetrics
               {
                   Id = wine.Id,
                   Name = wine.Name,
                   Winery = wine.Winery,
                   Year = wine.Year,
                   GrapeVariety = wine.GrapeVariety,
                   Description = wine.Description,
                   ImageUrl = wine.ImageUrl,
                   SecondaryImageUrl = wine.SecondaryImageUrl,
                   FeaturedReviewSummary = wine.FeaturedReviewSummary,
                   IsActive = wine.IsActive,
                   AverageRating = visibleReviews.Select(review => (decimal?)review.Rating).Average() ?? 0m,
                   ReviewsCount = visibleReviews.Count()
                };
    }

    private static IQueryable<WineWithMetrics> ApplyOrdering(IQueryable<WineWithMetrics> queryable, string sortBy, bool sortDescending)
    {
        return sortBy switch
        {
            "name" => sortDescending
                ? queryable.OrderByDescending(item => item.Name).ThenByDescending(item => item.Year).ThenBy(item => item.Id)
                : queryable.OrderBy(item => item.Name).ThenByDescending(item => item.Year).ThenBy(item => item.Id),
            "year" => sortDescending
                ? queryable.OrderByDescending(item => item.Year).ThenBy(item => item.Name).ThenBy(item => item.Id)
                : queryable.OrderBy(item => item.Year).ThenBy(item => item.Name).ThenBy(item => item.Id),
            _ => sortDescending
                ? queryable.OrderByDescending(item => item.AverageRating).ThenBy(item => item.Name).ThenBy(item => item.Id)
                : queryable.OrderBy(item => item.AverageRating).ThenBy(item => item.Name).ThenBy(item => item.Id)
        };
    }

    private static string EscapeLikePattern(string value)
    {
        return value
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("%", "\\%", StringComparison.Ordinal)
            .Replace("_", "\\_", StringComparison.Ordinal)
            .Replace("[", "\\[", StringComparison.Ordinal);
    }
}
