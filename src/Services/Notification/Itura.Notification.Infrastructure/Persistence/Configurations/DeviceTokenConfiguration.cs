using Itura.Notification.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.Notification.Infrastructure.Persistence.Configurations;

public sealed class DeviceTokenConfiguration : IEntityTypeConfiguration<DeviceToken>
{
    public void Configure(EntityTypeBuilder<DeviceToken> builder)
    {
        builder.ToTable("device_tokens");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(t => t.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(t => t.Token).HasColumnName("token").HasMaxLength(500).IsRequired();
        builder.Property(t => t.Platform).HasColumnName("platform").HasMaxLength(20).IsRequired();
        builder.Property(t => t.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(t => t.RegisteredAt).HasColumnName("registered_at");
        builder.Property(t => t.DeactivatedAt).HasColumnName("deactivated_at");
        builder.HasIndex(t => t.Token).IsUnique();
        builder.HasIndex(t => new { t.UserId, t.IsActive });
    }
}
