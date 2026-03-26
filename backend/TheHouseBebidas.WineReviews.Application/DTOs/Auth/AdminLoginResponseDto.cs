namespace TheHouseBebidas.WineReviews.Application.DTOs.Auth;

public sealed record AdminLoginResponseDto(
    string AccessToken,
    DateTime ExpiresAtUtc,
    string TokenType = "Bearer");
