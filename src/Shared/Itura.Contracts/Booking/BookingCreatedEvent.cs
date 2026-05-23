namespace Itura.Contracts.Booking;

public sealed record BookingCreatedEvent(
    Guid BookingId,
    Guid CoachId,
    Guid CoachUserId,
    Guid ClientUserId,
    DateTime ScheduledAt,
    int DurationMinutes,
    decimal Price,
    string Currency,
    DateTime CreatedAt);
