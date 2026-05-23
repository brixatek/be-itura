using Itura.Payment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.Payment.Infrastructure.Persistence.Configurations;

internal sealed class PaymentRecordConfiguration : IEntityTypeConfiguration<PaymentRecord>
{
    public void Configure(EntityTypeBuilder<PaymentRecord> builder)
    {
        builder.ToTable("payment_records");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(p => p.BookingId).HasColumnName("booking_id").IsRequired();
        builder.Property(p => p.PayerUserId).HasColumnName("payer_user_id").IsRequired();
        builder.Property(p => p.PayeeUserId).HasColumnName("payee_user_id").IsRequired();
        builder.Property(p => p.Amount).HasColumnName("amount").HasPrecision(18, 2).IsRequired();
        builder.Property(p => p.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired();
        builder.Property(p => p.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(p => p.PaymentMethod).HasColumnName("payment_method").HasMaxLength(50);
        builder.Property(p => p.TransactionReference).HasColumnName("transaction_reference").HasMaxLength(200);
        builder.Property(p => p.FailureReason).HasColumnName("failure_reason").HasMaxLength(500);
        builder.Property(p => p.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");
        builder.Property(p => p.DeletedAt).HasColumnName("deleted_at");

        builder.HasQueryFilter(p => !p.IsDeleted);

        builder.HasIndex(p => p.BookingId).IsUnique().HasDatabaseName("ix_payment_records_booking_id");
        builder.HasIndex(p => p.PayerUserId).HasDatabaseName("ix_payment_records_payer_user_id");
        builder.HasIndex(p => p.PayeeUserId).HasDatabaseName("ix_payment_records_payee_user_id");
        builder.HasIndex(p => p.Status).HasDatabaseName("ix_payment_records_status");
    }
}
