using TheHouseBebidas.WineReviews.Application.DTOs.Common;
using TheHouseBebidas.WineReviews.Application.DTOs.Wines;
using TheHouseBebidas.WineReviews.Application.Interfaces.Persistence;
using TheHouseBebidas.WineReviews.Application.Interfaces.Persistence.Models;
using TheHouseBebidas.WineReviews.Application.Interfaces.Public;

namespace TheHouseBebidas.WineReviews.Application.Services.Public;

public sealed class PublicWineService : IPublicWineService
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    private static readonly HashSet<string> AllowedSortFields =
    [
        "rating",
        "name",
        "year"
    ];

    private readonly IWineRepository _wineRepository;

    public PublicWineService(IWineRepository wineRepository)
    {
        _wineRepository = wineRepository;
    }

    public async Task<PaginatedResponseDto<WineListItemDto>> GetWinesAsync(WineListRequestDto request, CancellationToken cancellationToken = default)
    {
        var normalizedRequest = request ?? throw new ArgumentNullException(nameof(request));

        ValidateRatingsRange(normalizedRequest.MinimumRating, normalizedRequest.MaximumRating);

        var sortBy = NormalizeSortBy(normalizedRequest.SortBy);
        var sortDescending = string.IsNullOrWhiteSpace(normalizedRequest.SortBy)
            ? true
            : normalizedRequest.SortDescending;

        var page = NormalizePage(normalizedRequest.Page);
        var pageSize = NormalizePageSize(normalizedRequest.PageSize);

        var query = new WineListQuery(
            normalizedRequest.SearchTerm,
            normalizedRequest.MinimumRating,
            normalizedRequest.MaximumRating,
            sortBy,
            sortDescending,
            page,
            pageSize);

        var pagedWines = await _wineRepository.GetPublicWinesAsync(query, cancellationToken);

        var items = pagedWines.Items
            .Select(static wine => new WineListItemDto(
                wine.Id,
                wine.Name,
                wine.Winery,
                wine.Year,
                wine.GrapeVariety,
                wine.ImageUrl,
                wine.SecondaryImageUrl,
                wine.AverageRating,
                wine.ReviewsCount,
                wine.FeaturedReviewSummary))
            .ToArray();

        return new PaginatedResponseDto<WineListItemDto>(
            items,
            pagedWines.TotalItems,
            pagedWines.TotalPages,
            pagedWines.Page,
            pagedWines.PageSize);
    }

    public async Task<WineDetailDto?> GetWineDetailAsync(Guid wineId, CancellationToken cancellationToken = default)
    {
        if (wineId == Guid.Empty)
        {
            throw new ArgumentException("Wine id is required.", nameof(wineId));
        }

        var wine = await _wineRepository.GetPublicWineDetailByIdAsync(wineId, cancellationToken);

        return wine is null
            ? null
            : new WineDetailDto(
                wine.Id,
                wine.Name,
                wine.Winery,
                wine.Year,
                wine.GrapeVariety,
                wine.Description,
                wine.ImageUrl,
                wine.SecondaryImageUrl,
                wine.AverageRating,
                wine.ReviewsCount,
                wine.FeaturedReviewSummary,
                wine.IsActive);
    }

    private static void ValidateRatingsRange(int? minimumRating, int? maximumRating)
    {
        if (minimumRating is < 1 or > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(minimumRating), "Minimum rating must be between 1 and 5.");
        }

        if (maximumRating is < 1 or > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumRating), "Maximum rating must be between 1 and 5.");
        }

        if (minimumRating.HasValue && maximumRating.HasValue && minimumRating > maximumRating)
        {
            throw new ArgumentException("Minimum rating cannot be greater than maximum rating.", nameof(minimumRating));
        }
    }

    private static string NormalizeSortBy(string? sortBy)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return "rating";
        }

        var normalized = sortBy.Trim().ToLowerInvariant();

        if (!AllowedSortFields.Contains(normalized))
        {
            throw new ArgumentException("SortBy must be one of: rating, name, year.", nameof(sortBy));
        }

        return normalized;
    }

    private static int NormalizePage(int? page)
    {
        if (!page.HasValue)
        {
            return DefaultPage;
        }

        return Math.Max(DefaultPage, page.Value);
    }

    private static int NormalizePageSize(int? pageSize)
    {
        if (!pageSize.HasValue)
        {
            return DefaultPageSize;
        }

        return Math.Clamp(pageSize.Value, 1, MaxPageSize);
    }
}
