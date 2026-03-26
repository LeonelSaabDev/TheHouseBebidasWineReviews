namespace TheHouseBebidas.WineReviews.Application.DTOs.Wines;

public sealed record WineDetailDto(
    Guid Id,
    string Name,
    string Winery,
    int Year,
    string GrapeVariety,
    string Description,
    string ImageUrl,
    string? SecondaryImageUrl,
    decimal AverageRating,
    int ReviewsCount,
    string? FeaturedReviewSummary,
    bool IsActive);
