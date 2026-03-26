using Microsoft.Extensions.DependencyInjection;
using TheHouseBebidas.WineReviews.Application.Interfaces.Admin;
using TheHouseBebidas.WineReviews.Application.Interfaces.Auth;
using TheHouseBebidas.WineReviews.Application.Interfaces.Public;
using TheHouseBebidas.WineReviews.Application.Services.Admin;
using TheHouseBebidas.WineReviews.Application.Services.Auth;
using TheHouseBebidas.WineReviews.Application.Services.Public;

namespace TheHouseBebidas.WineReviews.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPublicWineService, PublicWineService>();
        services.AddScoped<IPublicReviewService, PublicReviewService>();
        services.AddScoped<IPublicSiteContentService, PublicSiteContentService>();
        services.AddScoped<IAdminAuthService, AdminAuthService>();
        services.AddScoped<IAdminWineService, AdminWineService>();
        services.AddScoped<IAdminReviewService, AdminReviewService>();
        services.AddScoped<IAdminSiteContentService, AdminSiteContentService>();

        return services;
    }
}
