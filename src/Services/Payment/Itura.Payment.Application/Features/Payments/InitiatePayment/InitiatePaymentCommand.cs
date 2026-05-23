using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Payment.Application.Features.Payments.InitiatePayment;

public sealed record InitiatePaymentCommand(
    Guid PayerUserId,
    Guid PayeeUserId,
    Guid BookingId,
    decimal Amount,
    string Currency,
    string? PaymentMethod) : IRequest<Result<Guid>>;
