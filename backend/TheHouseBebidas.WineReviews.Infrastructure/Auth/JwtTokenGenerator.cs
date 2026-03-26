using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TheHouseBebidas.WineReviews.Application.DTOs.Auth;
using TheHouseBebidas.WineReviews.Application.Interfaces.Auth;

namespace TheHouseBebidas.WineReviews.Infrastructure.Auth;

public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly string _issuer;
    private readonly string _audience;
    private readonly string _key;
    private readonly int _expirationMinutes;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _issuer = configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("Jwt:Issuer configuration is required.");
        _audience = configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("Jwt:Audience configuration is required.");
        _key = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key configuration is required.");

        if (!int.TryParse(configuration["Jwt:ExpirationMinutes"], out _expirationMinutes) || _expirationMinutes <= 0)
        {
            throw new InvalidOperationException("Jwt:ExpirationMinutes must be a positive integer.");
        }
    }

    public AdminLoginResponseDto GenerateToken(Guid adminUserId, string username)
    {
        if (adminUserId == Guid.Empty)
        {
            throw new ArgumentException("Admin user id is required.", nameof(adminUserId));
        }

        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username is required.", nameof(username));
        }

        var now = DateTime.UtcNow;
        var expiresAtUtc = now.AddMinutes(_expirationMinutes);
        var keyBytes = Encoding.UTF8.GetBytes(_key);
        var securityKey = new SymmetricSecurityKey(keyBytes);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, adminUserId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, username),
            new Claim(ClaimTypes.NameIdentifier, adminUserId.ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            notBefore: now,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new AdminLoginResponseDto(accessToken, expiresAtUtc);
    }
}
