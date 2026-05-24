namespace Itura.Contracts.Gamification;

public sealed record BadgeEarnedEvent(
    Guid UserId,
    string BadgeName,
    string BadgeDescription,
    string BadgeIconUrl,
    DateTime EarnedAt);
