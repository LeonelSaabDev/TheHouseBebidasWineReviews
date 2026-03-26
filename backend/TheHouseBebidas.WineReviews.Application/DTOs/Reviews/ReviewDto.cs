namespace TheHouseBebidas.WineReviews.Application.DTOs.Reviews;

public sealed record ReviewDto(
    Guid Id,
    Guid WineId,
    string AuthorName,
    string Comment,
    int Rating,
    DateTime CreatedAt,
    bool IsVisible);
