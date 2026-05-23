using Itura.Payment.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Payment.Application.Features.Payments.GetMyPayments;

public sealed record GetMyPaymentsQuery(
    Guid PayerUserId,
    int Page,
    int PageSize) : IRequest<Result<PagedResult<PaymentRecordDto>>>;
