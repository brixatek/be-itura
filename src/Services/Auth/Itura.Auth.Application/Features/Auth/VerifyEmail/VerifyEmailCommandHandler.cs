using Itura.Auth.Application.Common.Interfaces;
using Itura.Auth.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Auth.Application.Features.Auth.VerifyEmail;

internal sealed class VerifyEmailCommandHandler(
    IAccountRepository accountRepository,
    IAuthUnitOfWork unitOfWork) : IRequestHandler<VerifyEmailCommand, Result>
{
    public async Task<Result> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var accounts = await accountRepository.GetByVerifyTokenAsync(request.Token, cancellationToken);
        if (accounts is null)
            return Result.Failure(Error.NotFound("Account", request.Token));

        var result = accounts.VerifyEmail(request.Token);
        if (result.IsFailure) return result;

        accountRepository.Update(accounts);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
