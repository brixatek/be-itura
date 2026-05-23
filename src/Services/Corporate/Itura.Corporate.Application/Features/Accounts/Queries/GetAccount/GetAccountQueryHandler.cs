using Itura.Corporate.Application.DTOs;
using Itura.Corporate.Domain.Entities;
using Itura.Corporate.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Corporate.Application.Features.Accounts.Queries.GetAccount;

internal sealed class GetAccountQueryHandler(ICorporateAccountRepository repository)
    : IRequestHandler<GetAccountQuery, Result<CorporateAccountDto>>
{
    public async Task<Result<CorporateAccountDto>> Handle(GetAccountQuery request, CancellationToken cancellationToken)
    {
        var account = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (account is null)
            return Result.Failure<CorporateAccountDto>(Error.NotFound("CorporateAccount", request.Id));
        return ToDto(account);
    }

    internal static CorporateAccountDto ToDto(CorporateAccount a) => new(
        a.Id, a.CompanyName, a.AdminUserId, a.Industry,
        a.SubscriptionTier.ToString(), a.MaxEmployees, a.IsActive,
        a.LogoUrl, a.Website, a.CreatedAt, a.UpdatedAt);
}
