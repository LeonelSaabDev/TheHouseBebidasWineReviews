using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TheHouseBebidas.WineReviews.Application.Interfaces.Auth;
using TheHouseBebidas.WineReviews.Domain.Entities;

namespace TheHouseBebidas.WineReviews.Infrastructure.Persistence.Seeding;

public sealed class AdminUserSeeder
{
    private readonly WineReviewsDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AdminUserSeeder> _logger;

    public AdminUserSeeder(
        WineReviewsDbContext dbContext,
        IPasswordHasher passwordHasher,
        IConfiguration configuration,
        ILogger<AdminUserSeeder> logger)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var username = _configuration["AdminSeed:Username"]?.Trim();
        var password = _configuration["AdminSeed:Password"];
        var isActive = bool.TryParse(_configuration["AdminSeed:IsActive"], out var parsedIsActive)
            ? parsedIsActive
            : true;
        var rotatePasswordIfExists = bool.TryParse(
            _configuration["AdminSeed:RotatePasswordIfExists"],
            out var parsedRotatePasswordIfExists)
            && parsedRotatePasswordIfExists;

        if (string.IsNullOrWhiteSpace(username))
        {
            _logger.LogWarning("Admin seed skipped because AdminSeed:Username is missing.");
            return;
        }

        try
        {
            var existingUser = await _dbContext.AdminUsers
                .FirstOrDefaultAsync(user => user.Username == username, cancellationToken);

            if (existingUser is not null)
            {
                var hasChanges = false;

                if (existingUser.IsActive != isActive)
                {
                    if (isActive)
                    {
                        existingUser.Activate();
                    }
                    else
                    {
                        existingUser.Deactivate();
                    }

                    hasChanges = true;
                }

                if (rotatePasswordIfExists)
                {
                    if (string.IsNullOrWhiteSpace(password))
                    {
                        _logger.LogWarning(
                            "Admin seed could not rotate password for '{Username}' because AdminSeed:Password is missing.",
                            username);
                    }
                    else
                    {
                        var (rotatedPasswordHash, rotatedPasswordSalt) = _passwordHasher.HashPassword(password);
                        existingUser.UpdateCredentials(rotatedPasswordHash, rotatedPasswordSalt);
                        hasChanges = true;
                    }
                }

                if (hasChanges)
                {
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation(
                        "Admin seed updated existing admin '{Username}' (IsActive={IsActive}, PasswordRotated={PasswordRotated}).",
                        username,
                        isActive,
                        rotatePasswordIfExists && !string.IsNullOrWhiteSpace(password));
                }
                else
                {
                    _logger.LogInformation(
                        "Admin seed found existing admin '{Username}' with no changes required.",
                        username);
                }

                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                _logger.LogWarning(
                    "Admin seed skipped creation because AdminSeed:Password is missing for new user '{Username}'.",
                    username);
                return;
            }

            var (passwordHash, passwordSalt) = _passwordHasher.HashPassword(password);

            var adminUser = new AdminUser(
                id: Guid.Empty,
                username: username,
                passwordHash: passwordHash,
                passwordSalt: passwordSalt,
                isActive: isActive);

            await _dbContext.AdminUsers.AddAsync(adminUser, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Admin seed created for username '{Username}'.", username);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Admin seed skipped because database schema is not ready.");
        }
    }
}
