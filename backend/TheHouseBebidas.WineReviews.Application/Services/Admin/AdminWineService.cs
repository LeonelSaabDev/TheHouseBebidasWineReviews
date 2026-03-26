using TheHouseBebidas.WineReviews.Application.DTOs.Wines;
using TheHouseBebidas.WineReviews.Application.Interfaces.Admin;
using TheHouseBebidas.WineReviews.Application.Interfaces.Persistence;
using TheHouseBebidas.WineReviews.Domain.Entities;

namespace TheHouseBebidas.WineReviews.Application.Services.Admin;

public sealed class AdminWineService : IAdminWineService
{
    private readonly IWineRepository _wineRepository;

    public AdminWineService(IWineRepository wineRepository)
    {
        _wineRepository = wineRepository;
    }

    public async Task<WineDetailDto> CreateWineAsync(AdminCreateWineRequestDto request, CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var wine = new Wine(
            id: Guid.Empty,
            name: request.Name,
            winery: request.Winery,
            year: request.Year,
            grapeVariety: request.GrapeVariety,
            description: request.Description,
            imageUrl: request.ImageUrl,
            secondaryImageUrl: request.SecondaryImageUrl,
            featuredReviewSummary: request.FeaturedReviewSummary,
            isActive: request.IsActive);

        await _wineRepository.AddAsync(wine, cancellationToken);

        return await GetRequiredWineDetailAsync(wine.Id, cancellationToken);
    }

    public async Task<WineDetailDto?> UpdateWineAsync(Guid wineId, AdminUpdateWineRequestDto request, CancellationToken cancellationToken = default)
    {
        if (wineId == Guid.Empty)
        {
            throw new ArgumentException("Wine id is required.", nameof(wineId));
        }

        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var wine = await _wineRepository.GetByIdAsync(wineId, cancellationToken);

        if (wine is null)
        {
            return null;
        }

        wine.UpdateDetails(
            request.Name,
            request.Winery,
            request.Year,
            request.GrapeVariety,
            request.Description,
            request.ImageUrl,
            request.SecondaryImageUrl,
            request.IsActive);
        wine.SetFeaturedReviewSummary(request.FeaturedReviewSummary);

        await _wineRepository.SaveChangesAsync(cancellationToken);

        return await GetRequiredWineDetailAsync(wineId, cancellationToken);
    }

    public async Task<bool> DeleteWineAsync(Guid wineId, CancellationToken cancellationToken = default)
    {
        if (wineId == Guid.Empty)
        {
            throw new ArgumentException("Wine id is required.", nameof(wineId));
        }

        var wine = await _wineRepository.GetByIdAsync(wineId, cancellationToken);

        if (wine is null)
        {
            return false;
        }

        if (wine.IsActive)
        {
            wine.Deactivate();
            await _wineRepository.SaveChangesAsync(cancellationToken);
        }

        return true;
    }

    private async Task<WineDetailDto> GetRequiredWineDetailAsync(Guid wineId, CancellationToken cancellationToken)
    {
        var wineDetail = await _wineRepository.GetWineDetailByIdAsync(wineId, cancellationToken)
            ?? throw new InvalidOperationException("Wine detail could not be loaded after persistence.");

        return new WineDetailDto(
            wineDetail.Id,
            wineDetail.Name,
            wineDetail.Winery,
            wineDetail.Year,
            wineDetail.GrapeVariety,
            wineDetail.Description,
            wineDetail.ImageUrl,
            wineDetail.SecondaryImageUrl,
            wineDetail.AverageRating,
            wineDetail.ReviewsCount,
            wineDetail.FeaturedReviewSummary,
            wineDetail.IsActive);
    }
}
