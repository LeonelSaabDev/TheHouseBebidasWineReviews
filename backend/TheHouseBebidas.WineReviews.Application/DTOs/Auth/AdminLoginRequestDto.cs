namespace TheHouseBebidas.WineReviews.Application.DTOs.Auth;

public sealed record AdminLoginRequestDto(
    string Username,
    string Password);
