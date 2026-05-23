using Itura.Payment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.Payment.Infrastructure.Persistence.Configurations;

internal sealed class CoachBankAccountConfiguration : IEntityTypeConfiguration<CoachBankAccount>
{
    public void Configure(EntityTypeBuilder<CoachBankAccount> builder)
    {
        builder.ToTable("coach_bank_accounts");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(b => b.CoachUserId).HasColumnName("coach_user_id").IsRequired();
        builder.Property(b => b.BankCodeEncrypted).HasColumnName("bank_code_encrypted").HasMaxLength(500).IsRequired();
        builder.Property(b => b.AccountNumberEncrypted).HasColumnName("account_number_encrypted").HasMaxLength(500).IsRequired();
        builder.Property(b => b.AccountNameEncrypted).HasColumnName("account_name_encrypted").HasMaxLength(500).IsRequired();
        builder.Property(b => b.BankName).HasColumnName("bank_name").HasMaxLength(100).IsRequired();
        builder.Property(b => b.IsVerified).HasColumnName("is_verified").HasDefaultValue(false);
        builder.Property(b => b.CreatedAt).HasColumnName("created_at");
        builder.Property(b => b.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(b => b.CoachUserId).IsUnique().HasDatabaseName("ix_coach_bank_accounts_coach_user_id");
    }
}
