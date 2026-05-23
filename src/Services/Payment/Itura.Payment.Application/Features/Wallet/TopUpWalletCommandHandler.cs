using Itura.Payment.Application.Common.Interfaces;
using Itura.Payment.Application.DTOs;
using Itura.Payment.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Payment.Application.Features.Wallet;

internal sealed class TopUpWalletCommandHandler(
    IWalletRepository walletRepository,
    IPaymentUnitOfWork unitOfWork,
    IPaystackService paystack)
    : IRequestHandler<TopUpWalletCommand, Result<PaystackPaymentInitDto>>
{
    public async Task<Result<PaystackPaymentInitDto>> Handle(
        TopUpWalletCommand request, CancellationToken cancellationToken)
    {
        if (request.Amount <= 0)
            return Result.Failure<PaystackPaymentInitDto>(
                Error.Validation("Wallet.TopUp", "Top-up amount must be positive."));

        var wallet = await walletRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (wallet is null)
            return Result.Failure<PaystackPaymentInitDto>(
                Error.NotFound("Wallet.NotFound", "Wallet not found."));

        var reference = $"topup_{request.UserId:N}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
        var amountInSmallestUnit = (long)(request.Amount * 100);

        var initResult = await paystack.InitializeAsync(
            request.Email, amountInSmallestUnit, reference, request.CallbackUrl, cancellationToken);

        if (!initResult.Success || initResult.AuthorizationUrl is null)
            return Result.Failure<PaystackPaymentInitDto>(
                Error.Validation("Paystack.Init", initResult.Message ?? "Failed to initialize top-up."));

        // Store pending top-up transaction for tracking
        var pending = Domain.Entities.WalletTransaction.Create(
            wallet.Id, request.UserId, request.Amount,
            "pending_topup", "Wallet top-up (pending)", wallet.Balance, reference);

        await walletRepository.AddTransactionAsync(pending, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new PaystackPaymentInitDto(wallet.Id, initResult.AuthorizationUrl, reference));
    }
}
