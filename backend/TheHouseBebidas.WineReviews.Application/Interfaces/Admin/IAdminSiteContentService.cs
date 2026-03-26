using TheHouseBebidas.WineReviews.Application.DTOs.SiteContent;

namespace TheHouseBebidas.WineReviews.Application.Interfaces.Admin;

public interface IAdminSiteContentService
{
    Task<SiteContentDto?> UpdateSiteContentAsync(string key, UpdateSiteContentRequestDto request, CancellationToken cancellationToken = default);
}
