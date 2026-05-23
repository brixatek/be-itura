namespace Itura.Contracts.Booking;

public sealed record BookingConfirmedEvent(
    Guid BookingId,
    Guid CoachId,
    Guid ClientUserId,
    DateTime ScheduledAt,
    int DurationMinutes,
    decimal Price,
    string Currency,
    DateTime ConfirmedAt);
