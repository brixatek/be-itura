using Itura.Auth.Application.Common.Interfaces;
using Itura.Auth.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Auth.Application.Features.Admin;

internal sealed class SuspendAccountCommandHandler(
    IAccountRepository repository,
    IRefreshTokenRepository refreshTokenRepository,
    IAuthUnitOfWork unitOfWork)
    : IRequestHandler<SuspendAccountCommand, Result>,
      IRequestHandler<RestoreAccountCommand, Result>
{
    public async Task<Result> Handle(SuspendAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await repository.GetByIdAsync(request.AccountId, cancellationToken);
        if (account is null) return Result.Failure(Error.NotFound("Account", request.AccountId));

        var result = account.Suspend(request.Reason);
        if (result.IsFailure) return result;

        repository.Update(account);
        await refreshTokenRepository.RevokeAllForAccountAsync(request.AccountId, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> Handle(RestoreAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await repository.GetByIdAsync(request.AccountId, cancellationToken);
        if (account is null) return Result.Failure(Error.NotFound("Account", request.AccountId));

        var result = account.Restore();
        if (result.IsFailure) return result;

        repository.Update(account);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
