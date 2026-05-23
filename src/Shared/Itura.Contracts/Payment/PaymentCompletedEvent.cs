namespace Itura.Contracts.Payment;

public sealed record PaymentCompletedEvent(
    Guid PaymentId,
    Guid BookingId,
    Guid PayerUserId,
    Guid PayeeUserId,
    decimal Amount,
    string Currency,
    string? TransactionReference,
    DateTime CompletedAt);
