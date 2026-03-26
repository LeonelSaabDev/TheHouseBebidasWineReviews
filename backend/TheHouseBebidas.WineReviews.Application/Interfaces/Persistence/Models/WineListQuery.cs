namespace TheHouseBebidas.WineReviews.Application.Interfaces.Persistence.Models;

public sealed record WineListQuery(
    string? SearchTerm,
    int? MinimumRating,
    int? MaximumRating,
    string SortBy,
    bool SortDescending,
    int Page,
    int PageSize);
