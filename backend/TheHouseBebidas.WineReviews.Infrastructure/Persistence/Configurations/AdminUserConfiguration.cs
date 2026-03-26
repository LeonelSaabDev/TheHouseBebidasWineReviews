using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheHouseBebidas.WineReviews.Domain.Entities;

namespace TheHouseBebidas.WineReviews.Infrastructure.Persistence.Configurations;

public sealed class AdminUserConfiguration : IEntityTypeConfiguration<AdminUser>
{
    public void Configure(EntityTypeBuilder<AdminUser> builder)
    {
        builder.ToTable("AdminUsers");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Username)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(user => user.PasswordHash)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(user => user.PasswordSalt)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(user => user.IsActive)
            .IsRequired();

        builder.Property(user => user.CreatedAt)
            .IsRequired();

        builder.HasIndex(user => user.Username)
            .IsUnique();
    }
}
