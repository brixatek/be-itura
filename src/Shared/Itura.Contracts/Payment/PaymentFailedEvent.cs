namespace Itura.Contracts.Payment;

public sealed record PaymentFailedEvent(
    Guid PaymentId,
    Guid BookingId,
    Guid PayerUserId,
    decimal Amount,
    string Currency,
    string? FailureReason,
    DateTime FailedAt);
