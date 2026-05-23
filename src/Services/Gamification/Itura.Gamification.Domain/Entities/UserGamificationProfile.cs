using Itura.Gamification.Domain.Events;
using Itura.SharedKernel.Domain;
using Itura.SharedKernel.Results;

namespace Itura.Gamification.Domain.Entities;

public sealed class UserGamificationProfile : AggregateRoot
{
    public Guid UserId { get; private set; }
    public int TotalPoints { get; private set; }
    public int Level { get; private set; }
    public int CurrentStreak { get; private set; }
    public int LongestStreak { get; private set; }
    public DateTime? LastActivityDate { get; private set; }

    private UserGamificationProfile() { }

    public static UserGamificationProfile Create(Guid userId)
    {
        return new UserGamificationProfile
        {
            UserId = userId,
            TotalPoints = 0,
            Level = 1,
            CurrentStreak = 0,
            LongestStreak = 0
        };
    }

    public Result AwardPoints(int points, string reason)
    {
        if (points <= 0)
            return Result.Failure(Error.Validation("Gamification.Points", "Points must be greater than zero."));

        TotalPoints += points;
        Level = CalculateLevel(TotalPoints);
        LastActivityDate = DateTime.UtcNow;

        RaiseDomainEvent(new PointsAwardedDomainEvent(UserId, points, TotalPoints, Level, reason));
        return Result.Success();
    }

    public void UpdateStreak()
    {
        var today = DateTime.UtcNow.Date;
        if (LastActivityDate.HasValue && LastActivityDate.Value.Date == today.AddDays(-1))
        {
            CurrentStreak++;
            if (CurrentStreak > LongestStreak) LongestStreak = CurrentStreak;
        }
        else if (!LastActivityDate.HasValue || LastActivityDate.Value.Date < today.AddDays(-1))
        {
            CurrentStreak = 1;
        }
        LastActivityDate = DateTime.UtcNow;
    }

    private static int CalculateLevel(int points) => points / 100 + 1;
}
