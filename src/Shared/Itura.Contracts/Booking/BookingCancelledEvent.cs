namespace Itura.Contracts.Booking;

public sealed record BookingCancelledEvent(
    Guid BookingId,
    Guid CoachId,
    Guid ClientUserId,
    string? CancellationReason,
    DateTime CancelledAt);
