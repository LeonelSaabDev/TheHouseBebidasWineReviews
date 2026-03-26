using TheHouseBebidas.WineReviews.Application.DTOs.Reviews;
using TheHouseBebidas.WineReviews.Application.Interfaces.Admin;
using TheHouseBebidas.WineReviews.Application.Interfaces.Persistence;
using TheHouseBebidas.WineReviews.Application.Interfaces.Persistence.Models;
using TheHouseBebidas.WineReviews.Domain.Entities;

namespace TheHouseBebidas.WineReviews.Application.Services.Admin;

public sealed class AdminReviewService : IAdminReviewService
{
    private readonly IReviewRepository _reviewRepository;

    public AdminReviewService(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<IReadOnlyCollection<ReviewDto>> GetReviewsAsync(AdminReviewListRequestDto request, CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (request.WineId == Guid.Empty)
        {
            throw new ArgumentException("Wine id filter is invalid.", nameof(request.WineId));
        }

        if (request.Rating is < Review.MinimumRating or > Review.MaximumRating)
        {
            throw new ArgumentOutOfRangeException(nameof(request.Rating), $"Rating filter must be between {Review.MinimumRating} and {Review.MaximumRating}.");
        }

        if (request.CreatedFrom.HasValue && request.CreatedTo.HasValue && request.CreatedFrom > request.CreatedTo)
        {
            throw new ArgumentException("CreatedFrom cannot be greater than CreatedTo.", nameof(request.CreatedFrom));
        }

        var query = new AdminReviewListQuery(
            request.WineId,
            request.IsVisible,
            request.Rating,
            request.CreatedFrom,
            request.CreatedTo);

        var reviews = await _reviewRepository.GetAdminReviewsAsync(query, cancellationToken);

        return reviews
            .Select(static review => new ReviewDto(
                review.Id,
                review.WineId,
                review.AuthorName,
                review.Comment,
                review.Rating,
                review.CreatedAt,
                review.IsVisible))
            .ToArray();
    }

    public Task<bool> DeleteReviewAsync(Guid reviewId, CancellationToken cancellationToken = default)
    {
        if (reviewId == Guid.Empty)
        {
            throw new ArgumentException("Review id is required.", nameof(reviewId));
        }

        return _reviewRepository.DeleteByIdAsync(reviewId, cancellationToken);
    }
}
