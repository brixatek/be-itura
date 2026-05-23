using Itura.Payment.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Payment.Application.Features.Wallet;

public sealed record GetWalletTransactionsQuery(
    Guid UserId,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<PagedResult<WalletTransactionDto>>>;
