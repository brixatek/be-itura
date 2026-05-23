namespace Itura.Payment.Application.DTOs;

public sealed record PaystackPaymentInitDto(
    Guid PaymentId,
    string AuthorizationUrl,
    string Reference);
