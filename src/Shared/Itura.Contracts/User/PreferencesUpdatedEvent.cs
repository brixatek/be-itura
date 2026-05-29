namespace Itura.Contracts.User;

public sealed record PreferencesUpdatedEvent(
    Guid AccountId,
    bool EmailNotifications,
    bool PushNotifications,
    bool WeeklyDigest,
    string Theme,
    string Language,
    DateTime UpdatedAt);
