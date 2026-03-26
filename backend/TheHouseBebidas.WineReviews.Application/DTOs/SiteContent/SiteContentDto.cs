namespace TheHouseBebidas.WineReviews.Application.DTOs.SiteContent;

public sealed record SiteContentDto(
    Guid Id,
    string Key,
    string Title,
    string Content,
    DateTime UpdatedAt);
