using Itura.Payment.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Payment.Application.Features.Payments.Paystack;

public sealed record VerifyPaystackPaymentCommand(
    string Reference,
    Guid UserId) : IRequest<Result<PaymentRecordDto>>;
