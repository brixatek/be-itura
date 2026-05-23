using Itura.Payment.Application.DTOs;
using Itura.Payment.Application.Features.Payments.GetPayment;
using Itura.Payment.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Payment.Application.Features.Payments.GetPaymentByBooking;

internal sealed class GetPaymentByBookingQueryHandler(IPaymentRepository repository)
    : IRequestHandler<GetPaymentByBookingQuery, Result<PaymentRecordDto>>
{
    public async Task<Result<PaymentRecordDto>> Handle(GetPaymentByBookingQuery request, CancellationToken cancellationToken)
    {
        var payment = await repository.GetByBookingIdAsync(request.BookingId, cancellationToken);
        if (payment is null)
            return Result.Failure<PaymentRecordDto>(Error.NotFound("Payment", request.BookingId));

        if (payment.PayerUserId != request.UserId && payment.PayeeUserId != request.UserId)
            return Result.Failure<PaymentRecordDto>(Error.Forbidden());

        return Result.Success(GetPaymentQueryHandler.ToDto(payment));
    }
}
