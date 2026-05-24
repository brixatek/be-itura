using Itura.Notification.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.Notification.Infrastructure.Persistence.Configurations;

internal sealed class NotificationPreferenceConfiguration : IEntityTypeConfiguration<NotificationPreference>
{
    public void Configure(EntityTypeBuilder<NotificationPreference> builder)
    {
        builder.ToTable("notification_preferences");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(p => p.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(p => p.PushEnabled).HasColumnName("push_enabled").HasDefaultValue(true);
        builder.Property(p => p.EmailEnabled).HasColumnName("email_enabled").HasDefaultValue(true);
        builder.Property(p => p.QuietHoursStart).HasColumnName("quiet_hours_start");
        builder.Property(p => p.QuietHoursEnd).HasColumnName("quiet_hours_end");
        builder.Property(p => p.CreatedAt).HasColumnName("created_at");
        builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");
        builder.HasIndex(p => p.UserId).IsUnique().HasDatabaseName("ix_notification_preferences_user_id");
    }
}
