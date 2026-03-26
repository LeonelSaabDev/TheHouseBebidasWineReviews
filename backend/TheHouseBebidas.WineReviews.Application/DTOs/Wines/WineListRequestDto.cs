namespace TheHouseBebidas.WineReviews.Application.DTOs.Wines;

public sealed record WineListRequestDto(
    string? SearchTerm,
    int? MinimumRating,
    int? MaximumRating,
    string? SortBy,
    bool SortDescending = false,
    int? Page = null,
    int? PageSize = null);
