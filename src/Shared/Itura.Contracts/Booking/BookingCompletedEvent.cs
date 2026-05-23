namespace Itura.Contracts.Booking;

public sealed record BookingCompletedEvent(
    Guid BookingId,
    Guid CoachId,
    Guid ClientUserId,
    decimal Price,
    string Currency,
    DateTime CompletedAt);
