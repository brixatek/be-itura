namespace Itura.Contracts.Gamification;

public sealed record LevelUpEvent(
    Guid UserId,
    int OldLevel,
    int NewLevel,
    int TotalXp,
    DateTime AchievedAt);
