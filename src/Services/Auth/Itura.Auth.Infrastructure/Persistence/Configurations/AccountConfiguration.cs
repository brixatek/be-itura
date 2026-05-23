using Itura.Auth.Domain.Entities;
using Itura.Auth.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.Auth.Infrastructure.Persistence.Configurations;

public sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("accounts");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasColumnName("id").ValueGeneratedNever();

        builder.Property(a => a.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
        builder.HasIndex(a => a.Email).IsUnique();

        builder.Property(a => a.PasswordHash).HasColumnName("password_hash").HasMaxLength(255);
        builder.Property(a => a.EmailVerified).HasColumnName("email_verified").HasDefaultValue(false);
        builder.Property(a => a.EmailVerifyToken).HasColumnName("email_verify_token").HasMaxLength(100);
        builder.Property(a => a.EmailVerifyExpiry).HasColumnName("email_verify_expiry");

        builder.Property(a => a.AuthProvider)
            .HasColumnName("auth_provider")
            .HasMaxLength(20)
            .HasConversion<string>()
            .HasDefaultValue(AuthProvider.Local);

        builder.Property(a => a.OAuthProviderId).HasColumnName("oauth_provider_id").HasMaxLength(255);

        builder.Property(a => a.Role)
            .HasColumnName("role")
            .HasMaxLength(30)
            .HasConversion<string>()
            .HasDefaultValue(UserRole.User);

        builder.Property(a => a.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .HasConversion<string>()
            .HasDefaultValue(AccountStatus.Active);

        builder.Property(a => a.FailedLoginCount).HasColumnName("failed_login_count").HasDefaultValue(0);
        builder.Property(a => a.LockoutUntil).HasColumnName("lockout_until");
        builder.Property(a => a.LastLoginAt).HasColumnName("last_login_at");

        builder.Property(a => a.IsMfaEnabled).HasColumnName("is_mfa_enabled").HasDefaultValue(false);
        builder.Property(a => a.TotpSecretEncrypted).HasColumnName("totp_secret_encrypted");
        builder.Property(a => a.MfaBackupCodes).HasColumnName("mfa_backup_codes")
            .HasColumnType("text[]")
            .HasDefaultValueSql("'{}'::text[]");

        builder.Property(a => a.CreatedAt).HasColumnName("created_at");
        builder.Property(a => a.UpdatedAt).HasColumnName("updated_at");
        builder.Property(a => a.DeletedAt).HasColumnName("deleted_at");

        builder.HasQueryFilter(a => a.DeletedAt == null);

        builder.HasMany(a => a.RefreshTokens)
            .WithOne()
            .HasForeignKey(rt => rt.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.PasswordResetTokens)
            .WithOne()
            .HasForeignKey(prt => prt.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
