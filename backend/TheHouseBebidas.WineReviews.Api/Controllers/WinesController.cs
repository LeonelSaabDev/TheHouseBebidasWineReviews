using Microsoft.AspNetCore.Mvc;
using TheHouseBebidas.WineReviews.Application.DTOs.Common;
using TheHouseBebidas.WineReviews.Application.DTOs.Wines;
using TheHouseBebidas.WineReviews.Application.Interfaces.Public;

namespace TheHouseBebidas.WineReviews.Api.Controllers;

[ApiController]
[Route("api/wines")]
public sealed class WinesController : ControllerBase
{
    private readonly IPublicWineService _publicWineService;

    public WinesController(IPublicWineService publicWineService)
    {
        _publicWineService = publicWineService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponseDto<WineListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginatedResponseDto<WineListItemDto>>> GetWinesAsync(
        [FromQuery] string? searchTerm,
        [FromQuery] int? minimumRating,
        [FromQuery] int? maximumRating,
        [FromQuery] string? sortBy,
        [FromQuery] bool sortDescending = false,
        [FromQuery] int? page = null,
        [FromQuery] int? pageSize = null,
        CancellationToken cancellationToken = default)
    {
        var request = new WineListRequestDto(searchTerm, minimumRating, maximumRating, sortBy, sortDescending, page, pageSize);
        var wines = await _publicWineService.GetWinesAsync(request, cancellationToken);
        return Ok(wines);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(WineDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WineDetailDto>> GetWineByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var wine = await _publicWineService.GetWineDetailAsync(id, cancellationToken);

        return wine is null
            ? NotFound()
            : Ok(wine);
    }
}
