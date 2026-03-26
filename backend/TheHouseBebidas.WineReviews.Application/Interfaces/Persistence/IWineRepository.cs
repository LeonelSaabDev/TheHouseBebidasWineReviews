using TheHouseBebidas.WineReviews.Application.Interfaces.Persistence.Models;
using TheHouseBebidas.WineReviews.Domain.Entities;

namespace TheHouseBebidas.WineReviews.Application.Interfaces.Persistence;

public interface IWineRepository
{
    Task<PagedResult<WineListProjection>> GetPublicWinesAsync(WineListQuery query, CancellationToken cancellationToken = default);

    Task<WineDetailProjection?> GetPublicWineDetailByIdAsync(Guid wineId, CancellationToken cancellationToken = default);

    Task<WineDetailProjection?> GetWineDetailByIdAsync(Guid wineId, CancellationToken cancellationToken = default);

    Task<bool> ExistsActiveWineAsync(Guid wineId, CancellationToken cancellationToken = default);

    Task<Wine?> GetByIdAsync(Guid wineId, CancellationToken cancellationToken = default);

    Task AddAsync(Wine wine, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
