using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheHouseBebidas.WineReviews.Application.DTOs.SiteContent;
using TheHouseBebidas.WineReviews.Application.Interfaces.Admin;

namespace TheHouseBebidas.WineReviews.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/site-content")]
public sealed class AdminSiteContentController : ControllerBase
{
    private readonly IAdminSiteContentService _adminSiteContentService;

    public AdminSiteContentController(IAdminSiteContentService adminSiteContentService)
    {
        _adminSiteContentService = adminSiteContentService;
    }

    [HttpPut("{key}")]
    [ProducesResponseType(typeof(SiteContentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SiteContentDto>> UpdateAsync(
        string key,
        [FromBody] UpdateSiteContentRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var section = await _adminSiteContentService.UpdateSiteContentAsync(key, request, cancellationToken);

        return section is null
            ? NotFound()
            : Ok(section);
    }
}
