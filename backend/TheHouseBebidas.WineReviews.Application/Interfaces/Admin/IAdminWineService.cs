using TheHouseBebidas.WineReviews.Application.DTOs.Wines;

namespace TheHouseBebidas.WineReviews.Application.Interfaces.Admin;

public interface IAdminWineService
{
    Task<WineDetailDto> CreateWineAsync(AdminCreateWineRequestDto request, CancellationToken cancellationToken = default);

    Task<WineDetailDto?> UpdateWineAsync(Guid wineId, AdminUpdateWineRequestDto request, CancellationToken cancellationToken = default);

    Task<bool> DeleteWineAsync(Guid wineId, CancellationToken cancellationToken = default);
}
