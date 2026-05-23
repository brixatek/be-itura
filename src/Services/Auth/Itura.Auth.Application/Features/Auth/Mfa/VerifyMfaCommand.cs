using Itura.Auth.Application.Common.Interfaces;
using Itura.Auth.Application.DTOs;
using Itura.Auth.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;
using DomainRefreshToken = Itura.Auth.Domain.Entities.RefreshToken;

namespace Itura.Auth.Application.Features.Auth.Mfa;

/// <summary>Verifies TOTP code after a login that returned mfa_required.</summary>
public sealed record VerifyMfaCommand(
    Guid AccountId,
    string Code,
    bool UseBackupCode = false,
    string? DeviceInfo = null,
    string? IpAddress = null) : IRequest<Result<LoginResponseDto>>;

internal sealed class VerifyMfaCommandHandler(
    IAccountRepository accountRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IAuthUnitOfWork unitOfWork,
    IJwtService jwtService,
    ITotpService totp) : IRequestHandler<VerifyMfaCommand, Result<LoginResponseDto>>
{
    public async Task<Result<LoginResponseDto>> Handle(VerifyMfaCommand request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByIdAsync(request.AccountId, cancellationToken);
        if (account is null)
            return Result.Failure<LoginResponseDto>(Error.NotFound("Account", request.AccountId));

        if (!account.IsMfaEnabled || string.IsNullOrEmpty(account.TotpSecretEncrypted))
            return Result.Failure<LoginResponseDto>(Error.Validation("Auth.MfaNotEnabled", "MFA is not enabled."));

        bool valid;
        if (request.UseBackupCode)
        {
            var hashed = totp.HashBackupCode(request.Code);
            var useResult = account.UseBackupCode(hashed);
            valid = useResult.IsSuccess;
        }
        else
        {
            var secret = totp.DecryptSecret(account.TotpSecretEncrypted);
            valid = totp.Verify(secret, request.Code);
        }

        if (!valid)
            return Result.Failure<LoginResponseDto>(Error.Unauthorized("Invalid MFA code."));

        var loginResult = account.RecordSuccessfulLogin();
        if (loginResult.IsFailure) return Result.Failure<LoginResponseDto>(loginResult.Error);

        var jti = Guid.NewGuid().ToString();
        var accessToken = jwtService.GenerateAccessToken(account.Id, account.Email, account.Role.ToString(), jti);
        var (refreshRaw, refreshHash) = jwtService.GenerateRefreshToken();
        var refreshToken = DomainRefreshToken.Create(account.Id, refreshHash, jti, request.DeviceInfo, request.IpAddress);

        await refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        accountRepository.Update(account);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var tokens = new AuthTokensDto(accessToken, refreshRaw, 900);
        var user = new AuthUserDto(account.Id, account.Email, string.Empty, account.Role.ToString(), "Free", false, 1);
        return Result.Success(new LoginResponseDto(tokens, user));
    }
}
