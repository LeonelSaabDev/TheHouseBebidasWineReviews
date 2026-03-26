using TheHouseBebidas.WineReviews.Domain.Entities;

namespace TheHouseBebidas.WineReviews.Application.Interfaces.Persistence;

public interface IAdminUserRepository
{
    Task<AdminUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
}
