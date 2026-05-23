using Itura.Payment.Application.Common.Interfaces;
using Itura.Payment.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Payment.Application.Features.Payments.RefundPayment;

internal sealed class RefundPaymentCommandHandler(
    IPaymentRepository repository,
    IPaymentUnitOfWork unitOfWork)
    : IRequestHandler<RefundPaymentCommand, Result>
{
    public async Task<Result> Handle(RefundPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await repository.GetByIdAsync(request.PaymentId, cancellationToken);
        if (payment is null)
            return Result.Failure(Error.NotFound("Payment", request.PaymentId));

        if (payment.PayerUserId != request.UserId && payment.PayeeUserId != request.UserId)
            return Result.Failure(Error.Forbidden());

        var result = payment.Refund();
        if (result.IsFailure) return result;

        repository.Update(payment);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
