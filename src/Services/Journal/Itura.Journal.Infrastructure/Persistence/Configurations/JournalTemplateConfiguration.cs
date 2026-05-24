using Itura.Journal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.Journal.Infrastructure.Persistence.Configurations;

internal sealed class JournalTemplateConfiguration : IEntityTypeConfiguration<JournalTemplate>
{
    public void Configure(EntityTypeBuilder<JournalTemplate> builder)
    {
        builder.ToTable("journal_templates");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedNever();
        builder.Property(t => t.Name).HasMaxLength(100).IsRequired();
        builder.Property(t => t.Description).HasMaxLength(500).IsRequired();
        builder.Property(t => t.Prompt).HasMaxLength(2000).IsRequired();
        builder.Property(t => t.Category).HasMaxLength(50).IsRequired();

        builder.HasIndex(t => t.Category);

        // Seed default templates
        builder.HasData(
            JournalTemplate.Create(
                new Guid("00000000-0000-0000-0000-000000000001"),
                "Morning Intention",
                "Set a positive intention for the day",
                "What is one intention I want to set for today? How will I embody this intention in my actions?",
                "Mindfulness"),
            JournalTemplate.Create(
                new Guid("00000000-0000-0000-0000-000000000002"),
                "Gratitude Practice",
                "Reflect on things you're grateful for",
                "List three things I'm genuinely grateful for today. Why do these things matter to me?",
                "Gratitude"),
            JournalTemplate.Create(
                new Guid("00000000-0000-0000-0000-000000000003"),
                "Emotional Check-In",
                "Explore your current emotional state",
                "How am I feeling right now — emotionally and physically? What might be driving these feelings?",
                "Emotional Wellness"),
            JournalTemplate.Create(
                new Guid("00000000-0000-0000-0000-000000000004"),
                "Evening Reflection",
                "Review your day with compassion",
                "What went well today? What would I do differently? What did I learn about myself?",
                "Self-Reflection"),
            JournalTemplate.Create(
                new Guid("00000000-0000-0000-0000-000000000005"),
                "Stress Release",
                "Process stress and find perspective",
                "What is weighing on my mind right now? What is within my control, and what do I need to release?",
                "Stress Management"),
            JournalTemplate.Create(
                new Guid("00000000-0000-0000-0000-000000000006"),
                "Goal Alignment",
                "Connect with your bigger picture",
                "What goal or value am I working toward this week? What one small step can I take today to move forward?",
                "Goal Setting")
        );
    }
}
