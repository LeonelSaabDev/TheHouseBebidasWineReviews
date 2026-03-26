using TheHouseBebidas.WineReviews.Application.DTOs.Reviews;

namespace TheHouseBebidas.WineReviews.Application.Interfaces.Admin;

public interface IAdminReviewService
{
    Task<IReadOnlyCollection<ReviewDto>> GetReviewsAsync(AdminReviewListRequestDto request, CancellationToken cancellationToken = default);

    Task<bool> DeleteReviewAsync(Guid reviewId, CancellationToken cancellationToken = default);
}
