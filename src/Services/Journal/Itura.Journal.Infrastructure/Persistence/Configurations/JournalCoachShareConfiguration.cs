using Itura.Journal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.Journal.Infrastructure.Persistence.Configurations;

internal sealed class JournalCoachShareConfiguration : IEntityTypeConfiguration<JournalCoachShare>
{
    public void Configure(EntityTypeBuilder<JournalCoachShare> builder)
    {
        builder.ToTable("journal_coach_shares");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(s => s.JournalEntryId).HasColumnName("journal_entry_id").IsRequired();
        builder.Property(s => s.CoachId).HasColumnName("coach_id").IsRequired();
        builder.Property(s => s.SharedAt).HasColumnName("shared_at").IsRequired();
        builder.Property(s => s.RevokedAt).HasColumnName("revoked_at");
        builder.Property(s => s.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(s => s.UpdatedAt).HasColumnName("updated_at");
        builder.Property(s => s.DeletedAt).HasColumnName("deleted_at");

        builder.HasIndex(s => s.JournalEntryId).HasDatabaseName("ix_journal_coach_shares_entry_id");
        builder.HasIndex(s => s.CoachId).HasDatabaseName("ix_journal_coach_shares_coach_id");
        builder.HasIndex(s => new { s.JournalEntryId, s.CoachId })
            .HasDatabaseName("ix_journal_coach_shares_entry_coach");
    }
}
