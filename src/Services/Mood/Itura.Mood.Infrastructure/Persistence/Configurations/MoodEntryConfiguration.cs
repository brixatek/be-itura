using Itura.Mood.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.Mood.Infrastructure.Persistence.Configurations;

internal sealed class MoodEntryConfiguration : IEntityTypeConfiguration<MoodEntry>
{
    public void Configure(EntityTypeBuilder<MoodEntry> builder)
    {
        builder.ToTable("mood_entries");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(e => e.Score).HasColumnName("score").IsRequired();
        builder.Property(e => e.Note).HasColumnName("note").HasMaxLength(500);
        builder.Property(e => e.Emotions).HasColumnName("emotions")
            .HasColumnType("jsonb").IsRequired();
        builder.Property(e => e.LoggedAt).HasColumnName("logged_at").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        builder.Property(e => e.DeletedAt).HasColumnName("deleted_at");

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.HasIndex(e => e.UserId).HasDatabaseName("ix_mood_entries_user_id");
        builder.HasIndex(e => new { e.UserId, e.LoggedAt }).HasDatabaseName("ix_mood_entries_user_logged_at");
    }
}
