using Itura.Payment.Application.DTOs;
using Itura.Payment.Application.Features.Payments.GetPayment;
using Itura.Payment.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Payment.Application.Features.Payments.GetMyPayments;

internal sealed class GetMyPaymentsQueryHandler(IPaymentRepository repository)
    : IRequestHandler<GetMyPaymentsQuery, Result<PagedResult<PaymentRecordDto>>>
{
    public async Task<Result<PagedResult<PaymentRecordDto>>> Handle(GetMyPaymentsQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.GetByPayerUserIdAsync(request.PayerUserId, request.Page, request.PageSize, cancellationToken);
        var dtos = new PagedResult<PaymentRecordDto>(
            paged.Items.Select(GetPaymentQueryHandler.ToDto).ToList(),
            paged.TotalCount, paged.Page, paged.PageSize);
        return Result.Success(dtos);
    }
}
