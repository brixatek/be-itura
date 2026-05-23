using Itura.Payment.Application.DTOs;
using Itura.Payment.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Payment.Application.Features.Wallet;

internal sealed class GetWalletTransactionsQueryHandler(IWalletRepository repository)
    : IRequestHandler<GetWalletTransactionsQuery, Result<PagedResult<WalletTransactionDto>>>
{
    public async Task<Result<PagedResult<WalletTransactionDto>>> Handle(
        GetWalletTransactionsQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.GetTransactionsAsync(
            request.UserId, request.Page, request.PageSize, cancellationToken);

        var dtos = paged.Items.Select(t => new WalletTransactionDto(
            t.Id, t.Amount, t.Type, t.Description, t.Reference, t.BalanceAfter, t.CreatedAt)).ToList();

        return Result.Success(new PagedResult<WalletTransactionDto>(
            dtos, paged.TotalCount, paged.Page, paged.PageSize));
    }
}
