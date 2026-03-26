using TheHouseBebidas.WineReviews.Application.DTOs.SiteContent;

namespace TheHouseBebidas.WineReviews.Application.Interfaces.Public;

public interface IPublicSiteContentService
{
    Task<IReadOnlyCollection<SiteContentDto>> GetSiteContentAsync(CancellationToken cancellationToken = default);
}
