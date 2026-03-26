namespace TheHouseBebidas.WineReviews.Application.Interfaces.Persistence.Models;

public sealed record PagedResult<TItem>(
    IReadOnlyCollection<TItem> Items,
    int TotalItems,
    int TotalPages,
    int Page,
    int PageSize);
