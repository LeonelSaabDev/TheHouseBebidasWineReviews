using Microsoft.AspNetCore.Mvc;
using TheHouseBebidas.WineReviews.Application.DTOs.SiteContent;
using TheHouseBebidas.WineReviews.Application.Interfaces.Public;

namespace TheHouseBebidas.WineReviews.Api.Controllers;

[ApiController]
[Route("api/site-content")]
public sealed class SiteContentController : ControllerBase
{
    private readonly IPublicSiteContentService _publicSiteContentService;

    public SiteContentController(IPublicSiteContentService publicSiteContentService)
    {
        _publicSiteContentService = publicSiteContentService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<SiteContentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<SiteContentDto>>> GetAsync(CancellationToken cancellationToken = default)
    {
        var content = await _publicSiteContentService.GetSiteContentAsync(cancellationToken);
        return Ok(content);
    }
}
