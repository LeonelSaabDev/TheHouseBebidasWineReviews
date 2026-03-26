namespace TheHouseBebidas.WineReviews.Application.Interfaces.Persistence.Models;

public sealed record AdminReviewListQuery(
    Guid? WineId,
    bool? IsVisible,
    int? Rating,
    DateTime? CreatedFrom,
    DateTime? CreatedTo);
