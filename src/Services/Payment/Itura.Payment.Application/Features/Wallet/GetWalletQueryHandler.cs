using Itura.Payment.Application.DTOs;
using Itura.Payment.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Payment.Application.Features.Wallet;

internal sealed class GetWalletQueryHandler(IWalletRepository repository)
    : IRequestHandler<GetWalletQuery, Result<WalletDto>>
{
    public async Task<Result<WalletDto>> Handle(GetWalletQuery request, CancellationToken cancellationToken)
    {
        var wallet = await repository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (wallet is null)
            return Result.Failure<WalletDto>(Error.NotFound("Wallet.NotFound", "Wallet not found."));

        return Result.Success(new WalletDto(
            wallet.Id, wallet.UserId, wallet.Balance, wallet.SessionCredits, wallet.Currency));
    }
}
