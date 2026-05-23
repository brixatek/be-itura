using Itura.Media.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.Media.Infrastructure.Persistence.Configurations;

internal sealed class MediaAssetConfiguration : IEntityTypeConfiguration<MediaAsset>
{
    public void Configure(EntityTypeBuilder<MediaAsset> builder)
    {
        builder.ToTable("media_assets");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.FileName).IsRequired().HasMaxLength(255);
        builder.Property(e => e.StoredFileName).IsRequired().HasMaxLength(300);
        builder.Property(e => e.MimeType).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Url).IsRequired().HasMaxLength(500);

        builder.Property(e => e.MediaType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.HasIndex(e => e.UploaderUserId);
        builder.HasIndex(e => e.MediaType);
        builder.HasIndex(e => e.IsPublic);
        builder.HasIndex(e => e.CreatedAt);
    }
}
