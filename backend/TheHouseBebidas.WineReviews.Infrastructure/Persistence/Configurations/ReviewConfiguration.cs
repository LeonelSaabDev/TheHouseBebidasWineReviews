using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheHouseBebidas.WineReviews.Domain.Entities;

namespace TheHouseBebidas.WineReviews.Infrastructure.Persistence.Configurations;

public sealed class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews", static table =>
        {
            table.HasCheckConstraint("CK_Reviews_Rating_Range", $"[Rating] >= {Review.MinimumRating} AND [Rating] <= {Review.MaximumRating}");
            table.HasCheckConstraint("CK_Reviews_AuthorName_NotBlank", "LEN(LTRIM(RTRIM([AuthorName]))) > 0");
        });

        builder.HasKey(review => review.Id);

        builder.Property(review => review.WineId)
            .IsRequired();

        builder.Property(review => review.Comment)
            .IsRequired()
            .HasMaxLength(Review.MaximumCommentLength);

        builder.Property(review => review.AuthorName)
            .IsRequired()
            .HasMaxLength(Review.MaximumAuthorNameLength);

        builder.Property(review => review.Rating)
            .IsRequired();

        builder.Property(review => review.CreatedAt)
            .IsRequired();

        builder.Property(review => review.IsVisible)
            .IsRequired();

        builder.HasOne<Wine>()
            .WithMany()
            .HasForeignKey(review => review.WineId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(review => review.WineId);
        builder.HasIndex(review => new { review.WineId, review.IsVisible, review.CreatedAt });
    }
}
