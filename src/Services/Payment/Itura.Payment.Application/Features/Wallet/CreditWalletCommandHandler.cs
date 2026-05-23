using Itura.Payment.Application.Common.Interfaces;
using Itura.Payment.Domain.Entities;
using Itura.Payment.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Payment.Application.Features.Wallet;

internal sealed class CreditWalletCommandHandler(
    IWalletRepository walletRepository,
    IPaymentUnitOfWork unitOfWork)
    : IRequestHandler<CreditWalletCommand, Result>
{
    public async Task<Result> Handle(CreditWalletCommand request, CancellationToken cancellationToken)
    {
        var wallet = await walletRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (wallet is null)
            return Result.Failure(Error.NotFound("Wallet.NotFound", "Wallet not found."));

        var creditResult = wallet.Credit(request.Amount, request.Description);
        if (creditResult.IsFailure) return creditResult;

        var tx = WalletTransaction.Create(
            wallet.Id, request.UserId, request.Amount,
            "credit", request.Description, wallet.Balance, request.Reference);

        walletRepository.Update(wallet);
        await walletRepository.AddTransactionAsync(tx, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
