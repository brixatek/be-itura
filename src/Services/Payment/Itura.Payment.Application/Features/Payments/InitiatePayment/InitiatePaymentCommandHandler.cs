using Itura.Payment.Application.Common.Interfaces;
using Itura.Payment.Domain.Entities;
using Itura.Payment.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Payment.Application.Features.Payments.InitiatePayment;

internal sealed class InitiatePaymentCommandHandler(
    IPaymentRepository repository,
    IPaymentUnitOfWork unitOfWork)
    : IRequestHandler<InitiatePaymentCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(InitiatePaymentCommand request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByBookingIdAsync(request.BookingId, cancellationToken);
        if (existing is not null)
            return Result.Failure<Guid>(Error.Conflict("Payment.Duplicate", "A payment already exists for this booking."));

        var result = PaymentRecord.Initiate(
            request.BookingId, request.PayerUserId, request.PayeeUserId,
            request.Amount, request.Currency, request.PaymentMethod);

        if (result.IsFailure) return Result.Failure<Guid>(result.Error);

        await repository.AddAsync(result.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(result.Value.Id);
    }
}
