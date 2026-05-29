using Itura.SharedKernel.Domain;

namespace Itura.User.Domain.Events;

public sealed record PreferencesUpdatedDomainEvent(
    Guid EventId,
    DateTime OccurredAt,
    Guid AccountId,
    bool EmailNotifications,
    bool PushNotifications,
    bool WeeklyDigest,
    string Theme,
    string Language) : IDomainEvent
{
    public PreferencesUpdatedDomainEvent(
        Guid accountId, bool emailNotifications, bool pushNotifications,
        bool weeklyDigest, string theme, string language)
        : this(Guid.NewGuid(), DateTime.UtcNow, accountId,
            emailNotifications, pushNotifications, weeklyDigest, theme, language) { }
}
