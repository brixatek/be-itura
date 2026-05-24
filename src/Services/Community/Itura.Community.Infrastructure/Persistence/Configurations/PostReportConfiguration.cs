using Itura.Community.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.Community.Infrastructure.Persistence.Configurations;

internal sealed class PostReportConfiguration : IEntityTypeConfiguration<PostReport>
{
    public void Configure(EntityTypeBuilder<PostReport> builder)
    {
        builder.ToTable("post_reports");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(r => r.PostId).HasColumnName("post_id").IsRequired();
        builder.Property(r => r.ReporterUserId).HasColumnName("reporter_user_id").IsRequired();
        builder.Property(r => r.Reason).HasColumnName("reason").HasMaxLength(1000).IsRequired();
        builder.Property(r => r.IsReviewed).HasColumnName("is_reviewed").HasDefaultValue(false);
        builder.Property(r => r.ReviewedAt).HasColumnName("reviewed_at");
        builder.Property(r => r.CreatedAt).HasColumnName("created_at");
        builder.Property(r => r.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(r => r.PostId).HasDatabaseName("ix_post_reports_post_id");
        builder.HasIndex(r => new { r.PostId, r.ReporterUserId })
            .IsUnique()
            .HasDatabaseName("ix_post_reports_post_reporter_unique");
    }
}
