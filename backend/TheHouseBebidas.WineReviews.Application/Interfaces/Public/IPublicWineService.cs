using TheHouseBebidas.WineReviews.Application.DTOs.Common;
using TheHouseBebidas.WineReviews.Application.DTOs.Wines;

namespace TheHouseBebidas.WineReviews.Application.Interfaces.Public;

public interface IPublicWineService
{
    Task<PaginatedResponseDto<WineListItemDto>> GetWinesAsync(WineListRequestDto request, CancellationToken cancellationToken = default);

    Task<WineDetailDto?> GetWineDetailAsync(Guid wineId, CancellationToken cancellationToken = default);
}
