using Itura.Auth.Application.Common.Interfaces;
using Itura.Auth.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Auth.Application.Features.Auth.Mfa;

public sealed record VerifyMfaSetupCommand(Guid AccountId, string Code) : IRequest<Result<MfaBackupCodesDto>>;

public sealed record MfaBackupCodesDto(List<string> BackupCodes);

internal sealed class VerifyMfaSetupCommandHandler(
    IAccountRepository accountRepository,
    IAuthUnitOfWork unitOfWork,
    ITotpService totp) : IRequestHandler<VerifyMfaSetupCommand, Result<MfaBackupCodesDto>>
{
    public async Task<Result<MfaBackupCodesDto>> Handle(VerifyMfaSetupCommand request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByIdAsync(request.AccountId, cancellationToken);
        if (account is null)
            return Result.Failure<MfaBackupCodesDto>(Error.NotFound("Account", request.AccountId));

        if (account.IsMfaEnabled)
            return Result.Failure<MfaBackupCodesDto>(Error.Conflict("Auth.MfaAlreadyEnabled", "MFA is already enabled."));

        if (string.IsNullOrEmpty(account.TotpSecretEncrypted))
            return Result.Failure<MfaBackupCodesDto>(Error.Validation("Auth.MfaNotSetup", "Start MFA setup first."));

        var secret = totp.DecryptSecret(account.TotpSecretEncrypted);
        if (!totp.Verify(secret, request.Code))
            return Result.Failure<MfaBackupCodesDto>(Error.Validation("Auth.InvalidTotpCode", "Invalid TOTP code."));

        var rawCodes = totp.GenerateBackupCodes();
        var hashedCodes = rawCodes.Select(totp.HashBackupCode).ToList();

        var enableResult = account.EnableMfa(hashedCodes);
        if (enableResult.IsFailure) return Result.Failure<MfaBackupCodesDto>(enableResult.Error);

        accountRepository.Update(account);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new MfaBackupCodesDto(rawCodes));
    }
}
