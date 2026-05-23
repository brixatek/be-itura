using Itura.Payment.Application.DTOs;
using Itura.Payment.Domain.Entities;
using Itura.Payment.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Payment.Application.Features.Payments.GetPayment;

internal sealed class GetPaymentQueryHandler(IPaymentRepository repository)
    : IRequestHandler<GetPaymentQuery, Result<PaymentRecordDto>>
{
    public async Task<Result<PaymentRecordDto>> Handle(GetPaymentQuery request, CancellationToken cancellationToken)
    {
        var payment = await repository.GetByIdAsync(request.PaymentId, cancellationToken);
        if (payment is null)
            return Result.Failure<PaymentRecordDto>(Error.NotFound("Payment", request.PaymentId));

        if (payment.PayerUserId != request.UserId && payment.PayeeUserId != request.UserId)
            return Result.Failure<PaymentRecordDto>(Error.Forbidden());

        return Result.Success(ToDto(payment));
    }

    internal static PaymentRecordDto ToDto(PaymentRecord p) => new(
        p.Id, p.BookingId, p.PayerUserId, p.PayeeUserId,
        p.Amount, p.Currency, p.Status.ToString(),
        p.PaymentMethod, p.TransactionReference, p.FailureReason,
        p.CreatedAt, p.UpdatedAt);
}
