using Itura.Corporate.Application.DTOs;
using Itura.Corporate.Application.Features.Accounts.Queries.GetAccount;
using Itura.Corporate.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Corporate.Application.Features.Accounts.Queries.GetMyAccount;

internal sealed class GetMyAccountQueryHandler(ICorporateAccountRepository repository)
    : IRequestHandler<GetMyAccountQuery, Result<CorporateAccountDto>>
{
    public async Task<Result<CorporateAccountDto>> Handle(GetMyAccountQuery request, CancellationToken cancellationToken)
    {
        var account = await repository.GetByAdminUserIdAsync(request.AdminUserId, cancellationToken);
        if (account is null)
            return Result.Failure<CorporateAccountDto>(Error.NotFound("CorporateAccount", request.AdminUserId));
        return GetAccountQueryHandler.ToDto(account);
    }
}
