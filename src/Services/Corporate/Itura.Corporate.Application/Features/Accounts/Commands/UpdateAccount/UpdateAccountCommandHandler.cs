using Itura.Corporate.Application.Common.Interfaces;
using Itura.Corporate.Application.DTOs;
using Itura.Corporate.Application.Features.Accounts.Queries.GetAccount;
using Itura.Corporate.Domain.Enums;
using Itura.Corporate.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Corporate.Application.Features.Accounts.Commands.UpdateAccount;

internal sealed class UpdateAccountCommandHandler(
    ICorporateAccountRepository repository,
    ICorporateUnitOfWork unitOfWork)
    : IRequestHandler<UpdateAccountCommand, Result<CorporateAccountDto>>
{
    public async Task<Result<CorporateAccountDto>> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (account is null)
            return Result.Failure<CorporateAccountDto>(Error.NotFound("CorporateAccount", request.Id));

        if (account.AdminUserId != request.AdminUserId)
            return Result.Failure<CorporateAccountDto>(Error.Forbidden("Only the account admin can update this account."));

        if (!Enum.TryParse<SubscriptionTier>(request.SubscriptionTier, true, out var tier))
            return Result.Failure<CorporateAccountDto>(Error.Validation("CorporateAccount.SubscriptionTier", "Invalid subscription tier."));

        var result = account.Update(request.CompanyName, request.Industry, tier, request.MaxEmployees, request.LogoUrl, request.Website);
        if (result.IsFailure)
            return Result.Failure<CorporateAccountDto>(result.Error);

        repository.Update(account);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return GetAccountQueryHandler.ToDto(account);
    }
}
