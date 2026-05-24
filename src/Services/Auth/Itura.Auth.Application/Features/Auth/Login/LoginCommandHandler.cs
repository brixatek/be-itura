using Itura.Auth.Application.Common.Interfaces;
using Itura.Auth.Application.DTOs;
using Itura.Auth.Domain.Entities;
using Itura.Auth.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;
using DomainRefreshToken = Itura.Auth.Domain.Entities.RefreshToken;

namespace Itura.Auth.Application.Features.Auth.Login;

public sealed record LoginResult(LoginResponseDto? Response, MfaRequiredDto? MfaChallenge);

internal sealed class LoginCommandHandler(
    IAccountRepository accountRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IAuthAuditLogRepository auditLogRepository,
    IAuthUnitOfWork unitOfWork,
    IJwtService jwtService,
    IPasswordHasher passwordHasher) : IRequestHandler<LoginCommand, Result<LoginResult>>
{
    public async Task<Result<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (account is null || string.IsNullOrEmpty(account.PasswordHash) ||
            !passwordHasher.Verify(request.Password, account.PasswordHash))
        {
            account?.RecordFailedLogin();
            if (account is not null)
            {
                await auditLogRepository.AddAsync(AuthAuditLog.Create(
                    account.Id, "login", false, request.IpAddress, null, "Invalid credentials"), cancellationToken);
                accountRepository.Update(account);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
            return Result.Failure<LoginResult>(Error.Unauthorized("Invalid email or password."));
        }

        if (account.IsLockedOut() || account.Status != Itura.Auth.Domain.Enums.AccountStatus.Active)
        {
            return Result.Failure<LoginResult>(account.IsLockedOut()
                ? Error.Unauthorized($"Account is locked until {account.LockoutUntil:u}.")
                : Error.Forbidden("Account is suspended."));
        }

        // MFA gate — return challenge instead of tokens
        if (account.IsMfaEnabled)
        {
            account.RecordSuccessfulLogin(); // reset fail count without issuing token
            accountRepository.Update(account);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(new LoginResult(null, new MfaRequiredDto(true, account.Id)));
        }

        var loginResult = account.RecordSuccessfulLogin();
        if (loginResult.IsFailure) return Result.Failure<LoginResult>(loginResult.Error);

        var jti = Guid.NewGuid().ToString();
        var accessToken = jwtService.GenerateAccessToken(account.Id, account.Email, account.Role.ToString(), jti);
        var (refreshTokenRaw, refreshTokenHash) = jwtService.GenerateRefreshToken();

        var refreshToken = DomainRefreshToken.Create(
            account.Id, refreshTokenHash, jti, request.DeviceInfo, request.IpAddress);

        await refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await auditLogRepository.AddAsync(AuthAuditLog.Create(account.Id, "login", true, request.IpAddress), cancellationToken);
        accountRepository.Update(account);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var tokens = new AuthTokensDto(accessToken, refreshTokenRaw, 900);
        var user = new AuthUserDto(account.Id, account.Email, account.FullName, account.Role.ToString(), "Free", false, 1);
        return Result.Success(new LoginResult(new LoginResponseDto(tokens, user), null));
    }
}
