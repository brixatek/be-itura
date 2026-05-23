using Itura.Auth.Application.Common.Interfaces;
using Itura.Auth.Application.DTOs;
using Itura.Auth.Domain.Entities;
using Itura.Auth.Domain.Enums;
using Itura.Auth.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;
using DomainRefreshToken = Itura.Auth.Domain.Entities.RefreshToken;

namespace Itura.Auth.Application.Features.Auth.OAuth;

internal sealed class GoogleOAuthCommandHandler(
    IAccountRepository accountRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IAuthUnitOfWork unitOfWork,
    IGoogleOAuthService googleOAuth,
    IJwtService jwtService) : IRequestHandler<GoogleOAuthCommand, Result<LoginResponseDto>>
{
    public async Task<Result<LoginResponseDto>> Handle(GoogleOAuthCommand request, CancellationToken cancellationToken)
    {
        var userInfo = await googleOAuth.ExchangeCodeAsync(request.Code, request.RedirectUri, cancellationToken);

        if (userInfo is null)
            return Result.Failure<LoginResponseDto>(Error.Unauthorized("Failed to authenticate with Google."));

        if (!userInfo.EmailVerified)
            return Result.Failure<LoginResponseDto>(Error.Validation("OAuth.UnverifiedEmail", "Google account email is not verified."));

        var account = await accountRepository.GetByEmailAsync(userInfo.Email, cancellationToken);

        if (account is null)
        {
            account = Account.CreateOAuth(userInfo.Email, AuthProvider.Google, userInfo.ProviderId);
            await accountRepository.AddAsync(account, cancellationToken);
        }
        else if (account.AuthProvider == AuthProvider.Local)
        {
            // Existing password account — link OAuth provider
            account.RecordSuccessfulLogin();
            accountRepository.Update(account);
        }

        if (account.Status != AccountStatus.Active)
            return Result.Failure<LoginResponseDto>(Error.Forbidden("Account is suspended."));

        var jti = Guid.NewGuid().ToString();
        var accessToken = jwtService.GenerateAccessToken(account.Id, account.Email, account.Role.ToString(), jti);
        var (refreshTokenRaw, refreshTokenHash) = jwtService.GenerateRefreshToken();

        var refreshToken = DomainRefreshToken.Create(
            account.Id, refreshTokenHash, jti, request.DeviceInfo, request.IpAddress);

        await refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var tokens = new AuthTokensDto(accessToken, refreshTokenRaw, 900);
        var user = new AuthUserDto(account.Id, account.Email, userInfo.FullName ?? string.Empty, account.Role.ToString(), "Free", false, 1);
        return Result.Success(new LoginResponseDto(tokens, user));
    }
}
