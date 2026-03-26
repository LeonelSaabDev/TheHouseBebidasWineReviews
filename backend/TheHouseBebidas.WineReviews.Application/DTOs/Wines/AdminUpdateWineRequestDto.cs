namespace TheHouseBebidas.WineReviews.Application.DTOs.Wines;

public sealed record AdminUpdateWineRequestDto(
    string Name,
    string Winery,
    int Year,
    string GrapeVariety,
    string Description,
    string ImageUrl,
    string? SecondaryImageUrl,
    string? FeaturedReviewSummary,
    bool IsActive);
