using Itura.Auth.Application.Common.Interfaces;
using Itura.Auth.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Auth.Application.Features.Auth.Mfa;

public sealed record DisableMfaCommand(Guid AccountId, string Password) : IRequest<Result>;

internal sealed class DisableMfaCommandHandler(
    IAccountRepository accountRepository,
    IAuthUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher) : IRequestHandler<DisableMfaCommand, Result>
{
    public async Task<Result> Handle(DisableMfaCommand request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByIdAsync(request.AccountId, cancellationToken);
        if (account is null)
            return Result.Failure(Error.NotFound("Account", request.AccountId));

        if (!account.IsMfaEnabled)
            return Result.Failure(Error.Validation("Auth.MfaNotEnabled", "MFA is not enabled."));

        if (string.IsNullOrEmpty(account.PasswordHash) || !passwordHasher.Verify(request.Password, account.PasswordHash))
            return Result.Failure(Error.Unauthorized("Invalid password."));

        account.DisableMfa();
        accountRepository.Update(account);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
