namespace TheHouseBebidas.WineReviews.Application.Interfaces.Persistence.Models;

public sealed record WineDetailProjection(
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
