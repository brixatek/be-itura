using Itura.Auth.Application.Common.Interfaces;
using Itura.Auth.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Auth.Application.Features.Auth.DeleteAccount;

public sealed record DeleteAccountCommand(Guid AccountId, string Password) : IRequest<Result>;

internal sealed class DeleteAccountCommandHandler(
    IAccountRepository accountRepository,
    IAuthUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher) : IRequestHandler<DeleteAccountCommand, Result>
{
    public async Task<Result> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByIdAsync(request.AccountId, cancellationToken);
        if (account is null)
            return Result.Failure(Error.NotFound("Account", request.AccountId));

        // OAuth accounts have no password — skip password check
        if (!string.IsNullOrEmpty(account.PasswordHash) &&
            !passwordHasher.Verify(request.Password, account.PasswordHash))
            return Result.Failure(Error.Unauthorized("Invalid password."));

        var result = account.MarkForDeletion();
        if (result.IsFailure) return result;

        accountRepository.Update(account);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
