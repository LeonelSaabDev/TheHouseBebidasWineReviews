using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheHouseBebidas.WineReviews.Domain.Entities;

namespace TheHouseBebidas.WineReviews.Infrastructure.Persistence.Configurations;

public sealed class SiteContentConfiguration : IEntityTypeConfiguration<SiteContent>
{
    public void Configure(EntityTypeBuilder<SiteContent> builder)
    {
        builder.ToTable("SiteContents");

        builder.HasKey(content => content.Id);

        builder.Property(content => content.Key)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(content => content.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(content => content.Content)
            .IsRequired()
            .HasMaxLength(6000);

        builder.Property(content => content.UpdatedAt)
            .IsRequired();

        builder.HasIndex(content => content.Key)
            .IsUnique();
    }
}
