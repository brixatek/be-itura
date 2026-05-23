using Itura.Content.Domain.Entities;
using Itura.Content.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.Content.Infrastructure.Persistence.Configurations;

internal sealed class ContentItemConfiguration : IEntityTypeConfiguration<ContentItem>
{
    public void Configure(EntityTypeBuilder<ContentItem> builder)
    {
        builder.ToTable("content_items");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title).IsRequired().HasMaxLength(300);
        builder.Property(e => e.Slug).IsRequired().HasMaxLength(350);
        builder.Property(e => e.Description).HasMaxLength(1000);
        builder.Property(e => e.ThumbnailUrl).HasMaxLength(500);
        builder.Property(e => e.MediaUrl).HasMaxLength(500);

        builder.Property(e => e.ContentType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.Tags)
            .HasColumnType("text[]");

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.HasIndex(e => e.Slug).IsUnique();
        builder.HasIndex(e => e.AuthorUserId);
        builder.HasIndex(e => e.ContentType);
        builder.HasIndex(e => e.IsPublished);
        builder.HasIndex(e => e.CreatedAt);
    }
}
