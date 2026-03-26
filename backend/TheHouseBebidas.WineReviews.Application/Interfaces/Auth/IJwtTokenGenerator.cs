using TheHouseBebidas.WineReviews.Application.DTOs.Auth;

namespace TheHouseBebidas.WineReviews.Application.Interfaces.Auth;

public interface IJwtTokenGenerator
{
    AdminLoginResponseDto GenerateToken(Guid adminUserId, string username);
}
