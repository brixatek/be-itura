using Itura.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.Auth.Infrastructure.Persistence.Configurations;

public sealed class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
{
    public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
    {
        builder.ToTable("password_reset_tokens");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(t => t.AccountId).HasColumnName("account_id");
        builder.Property(t => t.TokenHash).HasColumnName("token_hash").HasMaxLength(255).IsRequired();
        builder.HasIndex(t => t.TokenHash).IsUnique();
        builder.Property(t => t.ExpiresAt).HasColumnName("expires_at");
        builder.Property(t => t.UsedAt).HasColumnName("used_at");
        builder.Property(t => t.CreatedAt).HasColumnName("created_at");

        builder.Ignore(t => t.IsExpired);
        builder.Ignore(t => t.IsUsed);
        builder.Ignore(t => t.IsValid);
    }
}
