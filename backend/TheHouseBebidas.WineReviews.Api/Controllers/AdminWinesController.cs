using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheHouseBebidas.WineReviews.Application.DTOs.Wines;
using TheHouseBebidas.WineReviews.Application.Interfaces.Admin;

namespace TheHouseBebidas.WineReviews.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/wines")]
public sealed class AdminWinesController : ControllerBase
{
    private readonly IAdminWineService _adminWineService;

    public AdminWinesController(IAdminWineService adminWineService)
    {
        _adminWineService = adminWineService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(WineDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<WineDetailDto>> CreateAsync(
        [FromBody] AdminCreateWineRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var wine = await _adminWineService.CreateWineAsync(request, cancellationToken);
        return Created($"/api/wines/{wine.Id}", wine);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(WineDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WineDetailDto>> UpdateAsync(
        Guid id,
        [FromBody] AdminUpdateWineRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var wine = await _adminWineService.UpdateWineAsync(id, request, cancellationToken);

        return wine is null
            ? NotFound()
            : Ok(wine);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var deleted = await _adminWineService.DeleteWineAsync(id, cancellationToken);

        return deleted
            ? NoContent()
            : NotFound();
    }
}
