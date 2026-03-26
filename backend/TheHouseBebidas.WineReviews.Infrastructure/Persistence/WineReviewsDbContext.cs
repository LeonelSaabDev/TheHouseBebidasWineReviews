using Microsoft.EntityFrameworkCore;
using TheHouseBebidas.WineReviews.Domain.Entities;

namespace TheHouseBebidas.WineReviews.Infrastructure.Persistence;

public sealed class WineReviewsDbContext : DbContext
{
    public WineReviewsDbContext(DbContextOptions<WineReviewsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Wine> Wines => Set<Wine>();

    public DbSet<Review> Reviews => Set<Review>();

    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();

    public DbSet<SiteContent> SiteContents => Set<SiteContent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WineReviewsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
