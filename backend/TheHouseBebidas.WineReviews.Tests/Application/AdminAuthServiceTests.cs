using TheHouseBebidas.WineReviews.Application.DTOs.Auth;
using TheHouseBebidas.WineReviews.Application.Interfaces.Auth;
using TheHouseBebidas.WineReviews.Application.Interfaces.Persistence;
using TheHouseBebidas.WineReviews.Application.Services.Auth;
using TheHouseBebidas.WineReviews.Domain.Entities;

namespace TheHouseBebidas.WineReviews.Tests.Application;

public sealed class AdminAuthServiceTests
{
    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        var admin = new AdminUser(
            id: Guid.NewGuid(),
            username: "admin.dev",
            passwordHash: "hash",
            passwordSalt: "salt",
            isActive: true);

        var expectedToken = new AdminLoginResponseDto("token-123", DateTime.UtcNow.AddMinutes(60));
        var repository = new FakeAdminUserRepository(admin);
        var hasher = new FakePasswordHasher(verifyResult: true);
        var tokenGenerator = new FakeJwtTokenGenerator(expectedToken);
        var service = new AdminAuthService(repository, hasher, tokenGenerator);

        var result = await service.LoginAsync(new AdminLoginRequestDto("admin.dev", "pass-ok"));

        Assert.Equal(expectedToken.AccessToken, result.AccessToken);
        Assert.Equal(expectedToken.TokenType, result.TokenType);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorized_WhenCredentialsAreInvalid()
    {
        var admin = new AdminUser(
            id: Guid.NewGuid(),
            username: "admin.dev",
            passwordHash: "hash",
            passwordSalt: "salt",
            isActive: true);

        var repository = new FakeAdminUserRepository(admin);
        var hasher = new FakePasswordHasher(verifyResult: false);
        var tokenGenerator = new FakeJwtTokenGenerator(new AdminLoginResponseDto("unused", DateTime.UtcNow));
        var service = new AdminAuthService(repository, hasher, tokenGenerator);

        var action = () => service.LoginAsync(new AdminLoginRequestDto("admin.dev", "bad-pass"));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(action);
    }

    private sealed class FakeAdminUserRepository : IAdminUserRepository
    {
        private readonly AdminUser? _adminUser;

        public FakeAdminUserRepository(AdminUser? adminUser)
        {
            _adminUser = adminUser;
        }

        public Task<AdminUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            if (_adminUser is null)
            {
                return Task.FromResult<AdminUser?>(null);
            }

            return Task.FromResult(_adminUser.Username == username ? _adminUser : null);
        }
    }

    private sealed class FakePasswordHasher : IPasswordHasher
    {
        private readonly bool _verifyResult;

        public FakePasswordHasher(bool verifyResult)
        {
            _verifyResult = verifyResult;
        }

        public (string Hash, string Salt) HashPassword(string password)
        {
            return ("hash", "salt");
        }

        public bool VerifyPassword(string password, string passwordHash, string passwordSalt)
        {
            return _verifyResult;
        }
    }

    private sealed class FakeJwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly AdminLoginResponseDto _response;

        public FakeJwtTokenGenerator(AdminLoginResponseDto response)
        {
            _response = response;
        }

        public AdminLoginResponseDto GenerateToken(Guid adminUserId, string username)
        {
            return _response;
        }
    }
}
