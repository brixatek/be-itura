using Itura.Payment.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Payment.Application.Features.Payments.GetPayment;

public sealed record GetPaymentQuery(Guid PaymentId, Guid UserId) : IRequest<Result<PaymentRecordDto>>;
