using Itura.SharedKernel.Entities;

namespace Itura.User.Domain.Entities;

public sealed class UserStreak : AuditableEntity
{
    public Guid UserProfileId { get; private set; }
    public string StreakType { get; private set; } = string.Empty;
    public int CurrentStreak { get; private set; }
    public int LongestStreak { get; private set; }
    public DateOnly LastActivityDate { get; private set; }

    private UserStreak() { }

    public static UserStreak Create(Guid userProfileId, string streakType, DateOnly date) =>
        new()
        {
            UserProfileId = userProfileId,
            StreakType = streakType,
            CurrentStreak = 1,
            LongestStreak = 1,
            LastActivityDate = date
        };

    public void RecordActivity(DateOnly date)
    {
        if (date <= LastActivityDate) return;

        if (date == LastActivityDate.AddDays(1))
            CurrentStreak++;
        else
            CurrentStreak = 1;

        if (CurrentStreak > LongestStreak)
            LongestStreak = CurrentStreak;

        LastActivityDate = date;
        MarkUpdated();
    }

    public bool IsAtRisk(DateOnly today) =>
        LastActivityDate == today.AddDays(-1);
}
