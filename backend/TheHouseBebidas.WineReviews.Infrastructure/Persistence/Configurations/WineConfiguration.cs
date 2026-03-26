using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheHouseBebidas.WineReviews.Domain.Entities;

namespace TheHouseBebidas.WineReviews.Infrastructure.Persistence.Configurations;

public sealed class WineConfiguration : IEntityTypeConfiguration<Wine>
{
    public void Configure(EntityTypeBuilder<Wine> builder)
    {
        builder.ToTable("Wines");

        builder.HasKey(wine => wine.Id);

        builder.Property(wine => wine.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(wine => wine.Winery)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(wine => wine.Year)
            .IsRequired();

        builder.Property(wine => wine.GrapeVariety)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(wine => wine.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(wine => wine.ImageUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(wine => wine.SecondaryImageUrl)
            .HasMaxLength(500);

        builder.Property(wine => wine.FeaturedReviewSummary)
            .HasMaxLength(300);

        builder.Property(wine => wine.IsActive)
            .IsRequired();

        builder.Property(wine => wine.CreatedAt)
            .IsRequired();

        builder.Property(wine => wine.UpdatedAt)
            .IsRequired();

        builder.HasIndex(wine => wine.IsActive);
        builder.HasIndex(wine => wine.Name);
        builder.HasIndex(wine => wine.Winery);
        builder.HasIndex(wine => wine.GrapeVariety);
    }
}
