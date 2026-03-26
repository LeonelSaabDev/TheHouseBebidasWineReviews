namespace TheHouseBebidas.WineReviews.Application.Interfaces.Persistence.Models;

public sealed record WineListProjection(
    Guid Id,
    string Name,
    string Winery,
    int Year,
    string GrapeVariety,
    string ImageUrl,
    string? SecondaryImageUrl,
    decimal AverageRating,
    int ReviewsCount,
    string? FeaturedReviewSummary);
