using Itura.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.Auth.Infrastructure.Persistence.Configurations;

public sealed class AuthAuditLogConfiguration : IEntityTypeConfiguration<AuthAuditLog>
{
    public void Configure(EntityTypeBuilder<AuthAuditLog> builder)
    {
        builder.ToTable("auth_audit_logs");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(l => l.AccountId).HasColumnName("account_id").IsRequired();
        builder.Property(l => l.EventType).HasColumnName("event_type").HasMaxLength(50).IsRequired();
        builder.Property(l => l.IpAddress).HasColumnName("ip_address").HasMaxLength(45);
        builder.Property(l => l.UserAgent).HasColumnName("user_agent").HasMaxLength(500);
        builder.Property(l => l.Success).HasColumnName("success");
        builder.Property(l => l.FailureReason).HasColumnName("failure_reason").HasMaxLength(255);
        builder.Property(l => l.OccurredAt).HasColumnName("occurred_at");
        builder.HasIndex(l => l.AccountId);
        builder.HasIndex(l => l.OccurredAt);
    }
}
