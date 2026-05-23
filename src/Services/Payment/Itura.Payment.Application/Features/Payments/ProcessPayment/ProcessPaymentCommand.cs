using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Payment.Application.Features.Payments.ProcessPayment;

public sealed record ProcessPaymentCommand(
    Guid PaymentId,
    Guid UserId,
    bool Succeed,
    string? TransactionReference,
    string? FailureReason) : IRequest<Result>;
