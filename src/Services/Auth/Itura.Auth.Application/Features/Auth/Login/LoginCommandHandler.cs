using Itura.Auth.Application.Common.Interfaces;
using Itura.Auth.Application.DTOs;
using Itura.Auth.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;
using DomainRefreshToken = Itura.Auth.Domain.Entities.RefreshToken;

namespace Itura.Auth.Application.Features.Auth.Login;

internal sealed class LoginCommandHandler(
    IAccountRepository accountRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IAuthUnitOfWork unitOfWork,
    IJwtService jwtService,
    IPasswordHasher passwordHasher) : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
{
    public async Task<Result<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (account is null || string.IsNullOrEmpty(account.PasswordHash) ||
            !passwordHasher.Verify(request.Password, account.PasswordHash))
        {
            account?.RecordFailedLogin();
            if (account is not null)
            {
                accountRepository.Update(account);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
            return Result.Failure<LoginResponseDto>(Error.Unauthorized("Invalid email or password."));
        }

        var loginResult = account.RecordSuccessfulLogin();
        if (loginResult.IsFailure) return Result.Failure<LoginResponseDto>(loginResult.Error);

        var jti = Guid.NewGuid().ToString();
        var accessToken = jwtService.GenerateAccessToken(account.Id, account.Email, account.Role.ToString(), jti);
        var (refreshTokenRaw, refreshTokenHash) = jwtService.GenerateRefreshToken();

        var refreshToken = DomainRefreshToken.Create(
            account.Id, refreshTokenHash, jti, request.DeviceInfo, request.IpAddress);

        await refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        accountRepository.Update(account);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var tokens = new AuthTokensDto(accessToken, refreshTokenRaw, 900);
        var user = new AuthUserDto(account.Id, account.Email, string.Empty, account.Role.ToString(), "Free", false, 1);

        return Result.Success(new LoginResponseDto(tokens, user));
    }
}
