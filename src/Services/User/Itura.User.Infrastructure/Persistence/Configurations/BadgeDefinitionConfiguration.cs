using Itura.User.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.User.Infrastructure.Persistence.Configurations;

internal sealed class BadgeDefinitionConfiguration : IEntityTypeConfiguration<BadgeDefinition>
{
    private static readonly (string Name, string Description, string Trigger, string Condition)[] BadgeSeed =
    [
        ("First Step",        "Complete your first journal entry",            "JournalCreated",        "journal_count >= 1"),
        ("Consistent Writer", "Write 7 journal entries",                      "JournalCreated",        "journal_count >= 7"),
        ("Story Teller",      "Write 30 journal entries",                     "JournalCreated",        "journal_count >= 30"),
        ("Mood Tracker",      "Log your mood for the first time",             "MoodLogged",            "mood_count >= 1"),
        ("Feelings Aware",    "Log mood 7 days in a row",                     "MoodLogged",            "mood_streak >= 7"),
        ("Emotional Master",  "Log mood 30 days in a row",                    "MoodLogged",            "mood_streak >= 30"),
        ("First Session",     "Complete your first coaching session",         "SessionCompleted",      "session_count >= 1"),
        ("Committed",         "Complete 5 coaching sessions",                 "SessionCompleted",      "session_count >= 5"),
        ("Dedicated",         "Complete 20 coaching sessions",                "SessionCompleted",      "session_count >= 20"),
        ("Wellness Starter",  "Complete onboarding",                          "OnboardingCompleted",   "onboarding_completed >= 1"),
        ("Goal Setter",       "Set at least 3 wellness goals",                "OnboardingCompleted",   "goal_count >= 3"),
        ("Week Warrior",      "Maintain a 7-day streak",                      "StreakUpdated",         "streak >= 7"),
        ("Month Master",      "Maintain a 30-day streak",                     "StreakUpdated",         "streak >= 30"),
        ("Century Club",      "Earn 100 XP",                                  "XpAwarded",             "xp >= 100"),
        ("High Achiever",     "Earn 1000 XP",                                 "XpAwarded",             "xp >= 1000"),
        ("Level 5",           "Reach wellness level 5",                       "LevelUp",               "level >= 5"),
        ("Level 10",          "Reach wellness level 10",                      "LevelUp",               "level >= 10"),
        ("Community Member",  "Create your first community post",             "CommunityPostCreated",  "post_count >= 1"),
        ("Voice of Reason",   "Get 10 reactions on a post",                   "CommunityPostCreated",  "reaction_received >= 10"),
        ("Early Adopter",     "Join during the first month of launch",        "Registration",          "registration_day <= 30"),
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

        var seed = BadgeSeed.Select((b, i) => BadgeDefinition.Create(
            new Guid($"00000000-0000-0000-0000-{(i + 1):D12}"),
            b.Name, b.Description, "/badges/default.svg", b.Trigger, b.Condition)).ToArray();

        builder.HasData(seed);
    }
}
