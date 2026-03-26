using TheHouseBebidas.WineReviews.Application.DTOs.Auth;
using TheHouseBebidas.WineReviews.Application.Interfaces.Auth;
using TheHouseBebidas.WineReviews.Application.Interfaces.Persistence;

namespace TheHouseBebidas.WineReviews.Application.Services.Auth;

public sealed class AdminAuthService : IAdminAuthService
{
    private readonly IAdminUserRepository _adminUserRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AdminAuthService(
        IAdminUserRepository adminUserRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _adminUserRepository = adminUserRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AdminLoginResponseDto> LoginAsync(AdminLoginRequestDto request, CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Username))
        {
            throw new ArgumentException("Username is required.", nameof(request.Username));
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException("Password is required.", nameof(request.Password));
        }

        var adminUser = await _adminUserRepository.GetByUsernameAsync(request.Username, cancellationToken);

        if (adminUser is null || !adminUser.IsActive)
        {
            throw new UnauthorizedAccessException("Invalid admin credentials.");
        }

        var isValidPassword = _passwordHasher.VerifyPassword(
            request.Password,
            adminUser.PasswordHash,
            adminUser.PasswordSalt);

        if (!isValidPassword)
        {
            throw new UnauthorizedAccessException("Invalid admin credentials.");
        }

        return _jwtTokenGenerator.GenerateToken(adminUser.Id, adminUser.Username);
    }
}
