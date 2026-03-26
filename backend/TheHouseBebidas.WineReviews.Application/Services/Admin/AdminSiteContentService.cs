using TheHouseBebidas.WineReviews.Application.DTOs.SiteContent;
using TheHouseBebidas.WineReviews.Application.Interfaces.Admin;
using TheHouseBebidas.WineReviews.Application.Interfaces.Persistence;

namespace TheHouseBebidas.WineReviews.Application.Services.Admin;

public sealed class AdminSiteContentService : IAdminSiteContentService
{
    private readonly ISiteContentRepository _siteContentRepository;

    public AdminSiteContentService(ISiteContentRepository siteContentRepository)
    {
        _siteContentRepository = siteContentRepository;
    }

    public async Task<SiteContentDto?> UpdateSiteContentAsync(string key, UpdateSiteContentRequestDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Content key is required.", nameof(key));
        }

        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var normalizedKey = key.Trim();
        var section = await _siteContentRepository.GetByKeyAsync(normalizedKey, cancellationToken);

        if (section is null)
        {
            return null;
        }

        section.Update(request.Title, request.Content);
        await _siteContentRepository.SaveChangesAsync(cancellationToken);

        return new SiteContentDto(
            section.Id,
            section.Key,
            section.Title,
            section.Content,
            section.UpdatedAt);
    }
}
