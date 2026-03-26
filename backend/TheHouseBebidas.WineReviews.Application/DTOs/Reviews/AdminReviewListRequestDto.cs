namespace TheHouseBebidas.WineReviews.Application.DTOs.Reviews;

public sealed record AdminReviewListRequestDto(
    Guid? WineId,
    bool? IsVisible,
    int? Rating,
    DateTime? CreatedFrom,
    DateTime? CreatedTo);
