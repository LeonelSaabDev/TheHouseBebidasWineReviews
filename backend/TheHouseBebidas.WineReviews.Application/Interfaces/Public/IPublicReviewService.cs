using TheHouseBebidas.WineReviews.Application.DTOs.Reviews;

namespace TheHouseBebidas.WineReviews.Application.Interfaces.Public;

public interface IPublicReviewService
{
    Task<IReadOnlyCollection<ReviewDto>> GetVisibleReviewsByWineAsync(Guid wineId, CancellationToken cancellationToken = default);

    Task<ReviewDto> CreateReviewAsync(Guid wineId, CreateReviewRequestDto request, CancellationToken cancellationToken = default);
}
