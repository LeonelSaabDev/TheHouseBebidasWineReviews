using TheHouseBebidas.WineReviews.Application.DTOs.SiteContent;
using TheHouseBebidas.WineReviews.Application.Interfaces.Persistence;
using TheHouseBebidas.WineReviews.Application.Interfaces.Public;

namespace TheHouseBebidas.WineReviews.Application.Services.Public;

public sealed class PublicSiteContentService : IPublicSiteContentService
{
    private readonly ISiteContentRepository _siteContentRepository;

    public PublicSiteContentService(ISiteContentRepository siteContentRepository)
    {
        _siteContentRepository = siteContentRepository;
    }

    public async Task<IReadOnlyCollection<SiteContentDto>> GetSiteContentAsync(CancellationToken cancellationToken = default)
    {
        var sections = await _siteContentRepository.GetAllAsync(cancellationToken);

        return sections
            .Select(static section => new SiteContentDto(
                section.Id,
                section.Key,
                section.Title,
                section.Content,
                section.UpdatedAt))
            .ToArray();
    }
}
