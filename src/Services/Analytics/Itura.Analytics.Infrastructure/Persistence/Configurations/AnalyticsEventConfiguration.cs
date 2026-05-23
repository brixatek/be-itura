using Itura.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.Analytics.Infrastructure.Persistence.Configurations;

internal sealed class AnalyticsEventConfiguration : IEntityTypeConfiguration<AnalyticsEvent>
{
    public void Configure(EntityTypeBuilder<AnalyticsEvent> builder)
    {
        builder.ToTable("analytics_events");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.EventType).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Source).HasMaxLength(100).IsRequired();
        builder.Property(e => e.PropertiesJson).HasColumnType("text");
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.OccurredAt);
        builder.HasIndex(e => e.EventType);
    }
}
