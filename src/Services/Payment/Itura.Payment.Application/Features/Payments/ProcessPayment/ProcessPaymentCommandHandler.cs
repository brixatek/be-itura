using Itura.Payment.Application.Common.Interfaces;
using Itura.Payment.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Payment.Application.Features.Payments.ProcessPayment;

internal sealed class ProcessPaymentCommandHandler(
    IPaymentRepository repository,
    IPaymentUnitOfWork unitOfWork)
    : IRequestHandler<ProcessPaymentCommand, Result>
{
    public async Task<Result> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await repository.GetByIdAsync(request.PaymentId, cancellationToken);
        if (payment is null)
            return Result.Failure(Error.NotFound("Payment", request.PaymentId));

        if (payment.PayerUserId != request.UserId)
            return Result.Failure(Error.Forbidden());

        var result = request.Succeed
            ? payment.Complete(request.TransactionReference)
            : payment.Fail(request.FailureReason);

        if (result.IsFailure) return result;

        repository.Update(payment);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
