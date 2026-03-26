namespace TheHouseBebidas.WineReviews.Application.DTOs.Wines;

public sealed record WineListItemDto(
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
