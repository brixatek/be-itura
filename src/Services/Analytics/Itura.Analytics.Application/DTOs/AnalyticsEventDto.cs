namespace Itura.Analytics.Application.DTOs;

public sealed record AnalyticsEventDto(
    Guid Id,
    Guid? UserId,
    string EventType,
    string Source,
    string? PropertiesJson,
    DateTime OccurredAt);
