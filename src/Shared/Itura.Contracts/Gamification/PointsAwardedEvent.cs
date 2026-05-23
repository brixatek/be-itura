namespace Itura.Contracts.Gamification;

public sealed record PointsAwardedEvent(
    Guid UserId,
    int Points,
    int NewTotal,
    int NewLevel,
    string Reason,
    DateTime AwardedAt);
