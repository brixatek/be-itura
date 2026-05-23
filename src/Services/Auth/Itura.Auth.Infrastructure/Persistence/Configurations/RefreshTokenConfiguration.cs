using Itura.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.Auth.Infrastructure.Persistence.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(t => t.AccountId).HasColumnName("account_id");
        builder.Property(t => t.TokenHash).HasColumnName("token_hash").HasMaxLength(255).IsRequired();
        builder.HasIndex(t => t.TokenHash).IsUnique();
        builder.Property(t => t.Jti).HasColumnName("jti").HasMaxLength(100).IsRequired();
        builder.Property(t => t.DeviceInfo).HasColumnName("device_info").HasMaxLength(500);
        builder.Property(t => t.IpAddress).HasColumnName("ip_address").HasMaxLength(45);
        builder.Property(t => t.ExpiresAt).HasColumnName("expires_at");
        builder.Property(t => t.RevokedAt).HasColumnName("revoked_at");
        builder.Property(t => t.ReplacedBy).HasColumnName("replaced_by");
        builder.Property(t => t.CreatedAt).HasColumnName("created_at");

        builder.Ignore(t => t.IsExpired);
        builder.Ignore(t => t.IsRevoked);
        builder.Ignore(t => t.IsActive);
    }
}
