using Itura.Payment.Application.Common.Interfaces;
using Itura.Payment.Application.DTOs;
using Itura.Payment.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Payment.Application.Features.Payments.Paystack;

internal sealed class VerifyPaystackPaymentCommandHandler(
    IPaymentRepository repository,
    IPaymentUnitOfWork unitOfWork,
    IPaystackService paystack)
    : IRequestHandler<VerifyPaystackPaymentCommand, Result<PaymentRecordDto>>
{
    public async Task<Result<PaymentRecordDto>> Handle(
        VerifyPaystackPaymentCommand request, CancellationToken cancellationToken)
    {
        var record = await repository.GetByReferenceAsync(request.Reference, cancellationToken);
        if (record is null)
            return Result.Failure<PaymentRecordDto>(Error.NotFound("Payment.NotFound", "Payment not found."));

        if (record.PayerUserId != request.UserId)
            return Result.Failure<PaymentRecordDto>(Error.Forbidden("Access denied."));

        var verifyResult = await paystack.VerifyAsync(request.Reference, cancellationToken);

        if (!verifyResult.Success)
            return Result.Failure<PaymentRecordDto>(
                Error.Validation("Paystack.Verify", verifyResult.Message ?? "Verification failed."));

        var opResult = verifyResult.Status == "success"
            ? record.Complete(request.Reference)
            : record.Fail($"Paystack status: {verifyResult.Status}");

        if (opResult.IsFailure && opResult.Error.Code != "Payment.Complete")
            return Result.Failure<PaymentRecordDto>(opResult.Error);

        repository.Update(record);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new PaymentRecordDto(
            record.Id, record.BookingId, record.PayerUserId, record.PayeeUserId,
            record.Amount, record.Currency, record.Status.ToString(),
            record.PaymentMethod, record.TransactionReference, record.FailureReason,
            record.CreatedAt, record.UpdatedAt));
    }
}
