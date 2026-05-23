namespace Itura.Payment.Application.DTOs;

public sealed record PaymentRecordDto(
    Guid Id,
    Guid BookingId,
    Guid PayerUserId,
    Guid PayeeUserId,
    decimal Amount,
    string Currency,
    string Status,
    string? PaymentMethod,
    string? TransactionReference,
    string? FailureReason,
    DateTime CreatedAt,
    DateTime UpdatedAt);
