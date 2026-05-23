using Itura.User.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.User.Infrastructure.Persistence.Configurations;

internal sealed class XpTransactionConfiguration : IEntityTypeConfiguration<XpTransaction>
{
    public void Configure(EntityTypeBuilder<XpTransaction> builder)
    {
        builder.ToTable("xp_transactions");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.UserProfileId).HasColumnName("user_profile_id").IsRequired();
        builder.Property(x => x.Amount).HasColumnName("amount").IsRequired();
        builder.Property(x => x.Action).HasColumnName("action").HasMaxLength(100).IsRequired();
        builder.Property(x => x.ReferenceId).HasColumnName("reference_id");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");

        builder.HasIndex(x => x.UserProfileId).HasDatabaseName("ix_xp_transactions_user_profile_id");
    }
}
