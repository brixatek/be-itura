using Itura.User.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.User.Infrastructure.Persistence.Configurations;

internal sealed class BadgeDefinitionConfiguration : IEntityTypeConfiguration<BadgeDefinition>
{
    private static readonly (string Name, string Description, string Trigger)[] BadgeSeed =
    [
        ("First Step", "Complete your first journal entry", "JournalCreated"),
        ("Consistent Writer", "Write 7 journal entries", "JournalCreated"),
        ("Story Teller", "Write 30 journal entries", "JournalCreated"),
        ("Mood Tracker", "Log your mood for the first time", "MoodLogged"),
        ("Feelings Aware", "Log mood 7 days in a row", "MoodLogged"),
        ("Emotional Master", "Log mood 30 days in a row", "MoodLogged"),
        ("First Session", "Complete your first coaching session", "SessionCompleted"),
        ("Committed", "Complete 5 coaching sessions", "SessionCompleted"),
        ("Dedicated", "Complete 20 coaching sessions", "SessionCompleted"),
        ("Wellness Starter", "Complete onboarding", "OnboardingCompleted"),
        ("Goal Setter", "Set at least 3 wellness goals", "OnboardingCompleted"),
        ("Week Warrior", "Maintain a 7-day streak", "StreakUpdated"),
        ("Month Master", "Maintain a 30-day streak", "StreakUpdated"),
        ("Century Club", "Earn 100 XP", "XpAwarded"),
        ("High Achiever", "Earn 1000 XP", "XpAwarded"),
        ("Level 5", "Reach wellness level 5", "LevelUp"),
        ("Level 10", "Reach wellness level 10", "LevelUp"),
        ("Community Member", "Create your first community post", "CommunityPostCreated"),
        ("Voice of Reason", "Get 10 reactions on a post", "CommunityPostCreated"),
        ("Early Adopter", "Join during the first month of launch", "Registration"),
    ];

    public void Configure(EntityTypeBuilder<BadgeDefinition> builder)
    {
        builder.ToTable("badge_definitions");

        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(b => b.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        builder.Property(b => b.Description).HasColumnName("description").HasMaxLength(500).IsRequired();
        builder.Property(b => b.IconUrl).HasColumnName("icon_url").HasMaxLength(500).IsRequired();
        builder.Property(b => b.Trigger).HasColumnName("trigger").HasMaxLength(100).IsRequired();
        builder.Property(b => b.Condition).HasColumnName("condition").HasMaxLength(500).IsRequired();

        builder.HasIndex(b => b.Name).IsUnique().HasDatabaseName("ix_badge_definitions_name");

        // Seed badge definitions
        var seed = BadgeSeed.Select((b, i) => BadgeDefinition.Create(
            new Guid($"00000000-0000-0000-0000-{(i + 1):D12}"),
            b.Name, b.Description, "/badges/default.svg", b.Trigger, "{}")).ToArray();

        builder.HasData(seed);
    }
}
