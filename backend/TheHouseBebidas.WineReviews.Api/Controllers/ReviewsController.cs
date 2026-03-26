using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using TheHouseBebidas.WineReviews.Application.DTOs.Reviews;
using TheHouseBebidas.WineReviews.Application.Interfaces.Public;

namespace TheHouseBebidas.WineReviews.Api.Controllers;

[ApiController]
[Route("api/wines/{wineId:guid}/reviews")]
public sealed class ReviewsController : ControllerBase
{
    private readonly IPublicReviewService _publicReviewService;

    public ReviewsController(IPublicReviewService publicReviewService)
    {
        _publicReviewService = publicReviewService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<ReviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<ReviewDto>>> GetByWineId(Guid wineId, CancellationToken cancellationToken = default)
    {
        var reviews = await _publicReviewService.GetVisibleReviewsByWineAsync(wineId, cancellationToken);
        return Ok(reviews);
    }

    [HttpPost]
    [EnableRateLimiting("PublicReviewCreatePolicy")]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<ReviewDto>> CreateAsync(
        Guid wineId,
        [FromBody] CreateReviewRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var review = await _publicReviewService.CreateReviewAsync(wineId, request, cancellationToken);
        return CreatedAtAction(nameof(GetByWineId), new { wineId }, review);
    }
}
