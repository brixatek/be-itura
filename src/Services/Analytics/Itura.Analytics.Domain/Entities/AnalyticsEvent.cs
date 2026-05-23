using Itura.SharedKernel.Domain;

namespace Itura.Analytics.Domain.Entities;

public sealed class AnalyticsEvent : AggregateRoot
{
    public Guid? UserId { get; private set; }
    public string EventType { get; private set; } = string.Empty;
    public string Source { get; private set; } = string.Empty;
    public string? PropertiesJson { get; private set; }
    public DateTime OccurredAt { get; private set; }

    private AnalyticsEvent() { }

    public static AnalyticsEvent Create(Guid? userId, string eventType, string source, string? propertiesJson = null)
    {
        return new AnalyticsEvent
        {
            UserId = userId,
            EventType = eventType,
            Source = source,
            PropertiesJson = propertiesJson,
            OccurredAt = DateTime.UtcNow
        };
    }
}
