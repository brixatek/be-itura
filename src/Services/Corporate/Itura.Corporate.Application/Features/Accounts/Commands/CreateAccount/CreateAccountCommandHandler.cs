using Itura.Corporate.Application.Common.Interfaces;
using Itura.Corporate.Application.DTOs;
using Itura.Corporate.Application.Features.Accounts.Queries.GetAccount;
using Itura.Corporate.Domain.Enums;
using Itura.Corporate.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Corporate.Application.Features.Accounts.Commands.CreateAccount;

internal sealed class CreateAccountCommandHandler(
    ICorporateAccountRepository repository,
    ICorporateUnitOfWork unitOfWork)
    : IRequestHandler<CreateAccountCommand, Result<CorporateAccountDto>>
{
    public async Task<Result<CorporateAccountDto>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<SubscriptionTier>(request.SubscriptionTier, true, out var tier))
            return Result.Failure<CorporateAccountDto>(Error.Validation("CorporateAccount.SubscriptionTier", "Invalid subscription tier."));

        var existing = await repository.GetByAdminUserIdAsync(request.AdminUserId, cancellationToken);
        if (existing is not null)
            return Result.Failure<CorporateAccountDto>(Error.Conflict("CorporateAccount.Conflict", "This user already has a corporate account."));

        var result = Domain.Entities.CorporateAccount.Create(
            request.CompanyName, request.AdminUserId, tier,
            request.MaxEmployees, request.Industry, request.LogoUrl, request.Website);

        if (result.IsFailure)
            return Result.Failure<CorporateAccountDto>(result.Error);

        await repository.AddAsync(result.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return GetAccountQueryHandler.ToDto(result.Value);
    }
}
