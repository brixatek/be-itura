namespace Itura.Contracts.Payment;

public sealed record PaymentRefundedEvent(
    Guid PaymentId,
    Guid BookingId,
    Guid PayerUserId,
    Guid PayeeUserId,
    decimal Amount,
    string Currency,
    DateTime RefundedAt);
