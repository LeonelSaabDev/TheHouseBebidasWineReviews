namespace TheHouseBebidas.WineReviews.Application.DTOs.Common;

public sealed record PaginatedResponseDto<TItem>(
    IReadOnlyCollection<TItem> Items,
    int TotalItems,
    int TotalPages,
    int Page,
    int PageSize);
