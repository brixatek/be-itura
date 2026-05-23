using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Payment.Application.Features.Payments.RefundPayment;

public sealed record RefundPaymentCommand(Guid PaymentId, Guid UserId) : IRequest<Result>;
