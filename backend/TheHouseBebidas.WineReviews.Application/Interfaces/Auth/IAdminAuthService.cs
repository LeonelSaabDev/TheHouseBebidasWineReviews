using TheHouseBebidas.WineReviews.Application.DTOs.Auth;

namespace TheHouseBebidas.WineReviews.Application.Interfaces.Auth;

public interface IAdminAuthService
{
    Task<AdminLoginResponseDto> LoginAsync(AdminLoginRequestDto request, CancellationToken cancellationToken = default);
}
