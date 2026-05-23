using Itura.Journal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.Journal.Infrastructure.Persistence.Configurations;

internal sealed class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> builder)
    {
        builder.ToTable("journal_entries");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(e => e.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
        builder.Property(e => e.Content).HasColumnName("content").IsRequired();
        builder.Property(e => e.Tags).HasColumnName("tags").HasColumnType("jsonb").IsRequired();
        builder.Property(e => e.MoodScore).HasColumnName("mood_score");
        builder.Property(e => e.IsPrivate).HasColumnName("is_private").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        builder.Property(e => e.DeletedAt).HasColumnName("deleted_at");

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.HasIndex(e => e.UserId).HasDatabaseName("ix_journal_entries_user_id");
        builder.HasIndex(e => new { e.UserId, e.CreatedAt }).HasDatabaseName("ix_journal_entries_user_created_at");
    }
}
