using TheHouseBebidas.WineReviews.Application.DTOs.Reviews;
using TheHouseBebidas.WineReviews.Application.Interfaces.Persistence;
using TheHouseBebidas.WineReviews.Application.Interfaces.Public;
using TheHouseBebidas.WineReviews.Domain.Entities;

namespace TheHouseBebidas.WineReviews.Application.Services.Public;

public sealed class PublicReviewService : IPublicReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IWineRepository _wineRepository;

    public PublicReviewService(IReviewRepository reviewRepository, IWineRepository wineRepository)
    {
        _reviewRepository = reviewRepository;
        _wineRepository = wineRepository;
    }

    public async Task<IReadOnlyCollection<ReviewDto>> GetVisibleReviewsByWineAsync(Guid wineId, CancellationToken cancellationToken = default)
    {
        await EnsureWineExistsAsync(wineId, cancellationToken);

        var reviews = await _reviewRepository.GetVisibleByWineIdAsync(wineId, cancellationToken);

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

    public async Task<ReviewDto> CreateReviewAsync(Guid wineId, CreateReviewRequestDto request, CancellationToken cancellationToken = default)
    {
        await EnsureWineExistsAsync(wineId, cancellationToken);

        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var review = new Review(
            id: Guid.Empty,
            wineId: wineId,
            comment: request.Comment,
            authorName: request.AuthorName,
            rating: request.Rating,
            isVisible: true);

        await _reviewRepository.AddAsync(review, cancellationToken);

        return new ReviewDto(
            review.Id,
            review.WineId,
            review.AuthorName,
            review.Comment,
            review.Rating,
            review.CreatedAt,
            review.IsVisible);
    }

    private async Task EnsureWineExistsAsync(Guid wineId, CancellationToken cancellationToken)
    {
        if (wineId == Guid.Empty)
        {
            throw new ArgumentException("Wine id is required.", nameof(wineId));
        }

        var exists = await _wineRepository.ExistsActiveWineAsync(wineId, cancellationToken);

        if (!exists)
        {
            throw new KeyNotFoundException("Wine was not found.");
        }
    }
}
