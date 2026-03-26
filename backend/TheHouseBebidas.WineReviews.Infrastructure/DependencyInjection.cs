using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheHouseBebidas.WineReviews.Application.Interfaces.Auth;
using TheHouseBebidas.WineReviews.Application.Interfaces.Persistence;
using TheHouseBebidas.WineReviews.Infrastructure.Auth;
using TheHouseBebidas.WineReviews.Infrastructure.Persistence;
using TheHouseBebidas.WineReviews.Infrastructure.Persistence.Seeding;
using TheHouseBebidas.WineReviews.Infrastructure.Persistence.Repositories;

namespace TheHouseBebidas.WineReviews.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        services.AddDbContext<WineReviewsDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IWineRepository, EfWineRepository>();
        services.AddScoped<IReviewRepository, EfReviewRepository>();
        services.AddScoped<ISiteContentRepository, EfSiteContentRepository>();
        services.AddScoped<IAdminUserRepository, EfAdminUserRepository>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<AdminUserSeeder>();
        services.AddScoped<DevelopmentSampleDataSeeder>();

        return services;
    }
}
