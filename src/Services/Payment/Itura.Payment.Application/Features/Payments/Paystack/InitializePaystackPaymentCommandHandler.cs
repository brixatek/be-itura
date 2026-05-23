using Itura.Payment.Application.Common.Interfaces;
using Itura.Payment.Application.DTOs;
using Itura.Payment.Domain.Entities;
using Itura.Payment.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Payment.Application.Features.Payments.Paystack;

internal sealed class InitializePaystackPaymentCommandHandler(
    IPaymentRepository repository,
    IPaymentUnitOfWork unitOfWork,
    IPaystackService paystack)
    : IRequestHandler<InitializePaystackPaymentCommand, Result<PaystackPaymentInitDto>>
{
    public async Task<Result<PaystackPaymentInitDto>> Handle(
        InitializePaystackPaymentCommand request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByBookingIdAsync(request.BookingId, cancellationToken);
        if (existing is not null)
            return Result.Failure<PaystackPaymentInitDto>(
                Error.Conflict("Payment.Duplicate", "A payment already exists for this booking."));

        var initResult = PaymentRecord.Initiate(
            request.BookingId, request.PayerUserId, request.PayeeUserId,
            request.Amount, request.Currency, "paystack");

        if (initResult.IsFailure)
            return Result.Failure<PaystackPaymentInitDto>(initResult.Error);

        var record = initResult.Value;
        var reference = $"itura_{record.Id:N}";

        // Paystack uses smallest currency unit (kobo for NGN = amount × 100)
        var amountInSmallestUnit = (long)(request.Amount * 100);

        var paystackResult = await paystack.InitializeAsync(
            request.Email, amountInSmallestUnit, reference, request.CallbackUrl, cancellationToken);

        if (!paystackResult.Success || paystackResult.AuthorizationUrl is null)
            return Result.Failure<PaystackPaymentInitDto>(
                Error.Validation("Paystack.Init", paystackResult.Message ?? "Failed to initialize payment with Paystack."));

        record.AssignReference(reference);

        await repository.AddAsync(record, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new PaystackPaymentInitDto(
            record.Id, paystackResult.AuthorizationUrl, reference));
    }
}
