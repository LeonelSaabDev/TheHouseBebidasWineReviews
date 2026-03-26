using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheHouseBebidas.WineReviews.Application.DTOs.Reviews;
using TheHouseBebidas.WineReviews.Application.Interfaces.Admin;

namespace TheHouseBebidas.WineReviews.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/reviews")]
public sealed class AdminReviewsController : ControllerBase
{
    private readonly IAdminReviewService _adminReviewService;

    public AdminReviewsController(IAdminReviewService adminReviewService)
    {
        _adminReviewService = adminReviewService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<ReviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyCollection<ReviewDto>>> GetAsync(
        [FromQuery] Guid? wineId,
        [FromQuery] bool? isVisible,
        [FromQuery] int? rating,
        [FromQuery] DateTime? createdFrom,
        [FromQuery] DateTime? createdTo,
        CancellationToken cancellationToken = default)
    {
        var request = new AdminReviewListRequestDto(wineId, isVisible, rating, createdFrom, createdTo);
        var reviews = await _adminReviewService.GetReviewsAsync(request, cancellationToken);
        return Ok(reviews);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var deleted = await _adminReviewService.DeleteReviewAsync(id, cancellationToken);

        return deleted
            ? NoContent()
            : NotFound();
    }
}
