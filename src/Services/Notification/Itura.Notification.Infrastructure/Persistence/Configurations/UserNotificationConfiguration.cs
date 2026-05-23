using Itura.Notification.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.Notification.Infrastructure.Persistence.Configurations;

internal sealed class UserNotificationConfiguration : IEntityTypeConfiguration<UserNotification>
{
    public void Configure(EntityTypeBuilder<UserNotification> builder)
    {
        builder.ToTable("notifications");

        builder.HasKey(n => n.Id);
        builder.Property(n => n.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(n => n.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(n => n.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
        builder.Property(n => n.Body).HasColumnName("body").HasMaxLength(2000).IsRequired();
        builder.Property(n => n.Type).HasColumnName("type").HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(n => n.Channel).HasColumnName("channel").HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(n => n.IsRead).HasColumnName("is_read").IsRequired();
        builder.Property(n => n.ReadAt).HasColumnName("read_at");
        builder.Property(n => n.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(n => n.UpdatedAt).HasColumnName("updated_at");
        builder.Ignore(n => n.DeletedAt);
        builder.Ignore(n => n.IsDeleted);

        builder.HasIndex(n => n.UserId).HasDatabaseName("ix_notifications_user_id");
        builder.HasIndex(n => new { n.UserId, n.IsRead }).HasDatabaseName("ix_notifications_user_id_is_read");
    }
}
