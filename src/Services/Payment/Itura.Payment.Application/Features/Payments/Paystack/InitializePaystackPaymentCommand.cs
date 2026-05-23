using Itura.Payment.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Payment.Application.Features.Payments.Paystack;

public sealed record InitializePaystackPaymentCommand(
    Guid PayerUserId,
    Guid PayeeUserId,
    Guid BookingId,
    decimal Amount,
    string Currency,
    string Email,
    string? CallbackUrl = null) : IRequest<Result<PaystackPaymentInitDto>>;
