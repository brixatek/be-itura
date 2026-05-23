using Itura.Auth.Application.Common.Interfaces;
using Itura.Auth.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Auth.Application.Features.Auth.Mfa;

public sealed record SetupMfaCommand(Guid AccountId) : IRequest<Result<MfaSetupDto>>;

public sealed record MfaSetupDto(string Secret, string OtpAuthUri);

internal sealed class SetupMfaCommandHandler(
    IAccountRepository accountRepository,
    IAuthUnitOfWork unitOfWork,
    ITotpService totp) : IRequestHandler<SetupMfaCommand, Result<MfaSetupDto>>
{
    public async Task<Result<MfaSetupDto>> Handle(SetupMfaCommand request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByIdAsync(request.AccountId, cancellationToken);
        if (account is null)
            return Result.Failure<MfaSetupDto>(Error.NotFound("Account", request.AccountId));

        if (account.IsMfaEnabled)
            return Result.Failure<MfaSetupDto>(Error.Conflict("Auth.MfaAlreadyEnabled", "MFA is already enabled."));

        var secret = totp.GenerateSecret();
        var base32 = totp.ToBase32(secret);
        var encrypted = totp.EncryptSecret(secret);

        account.StoreTotpSecret(encrypted);
        accountRepository.Update(account);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new MfaSetupDto(base32, totp.BuildUri(account.Email, base32)));
    }
}
