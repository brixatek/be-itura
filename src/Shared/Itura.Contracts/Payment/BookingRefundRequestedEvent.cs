namespace Itura.Contracts.Payment;

public sealed record BookingRefundRequestedEvent(
    Guid BookingId,
    Guid ClientUserId,
    Guid CoachUserId,
    decimal RefundAmount,
    string Currency,
    string RefundTier,
    DateTime CancelledAt);
