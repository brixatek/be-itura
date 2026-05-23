using Itura.Auth.Application.Common.Interfaces;
using Itura.Auth.Application.DTOs;
using Itura.Auth.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;
using DomainRefreshToken = Itura.Auth.Domain.Entities.RefreshToken;

namespace Itura.Auth.Application.Features.Auth.VerifyEmail;

internal sealed class VerifyEmailCommandHandler(
    IAccountRepository accountRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IAuthUnitOfWork unitOfWork,
    IJwtService jwtService) : IRequestHandler<VerifyEmailCommand, Result<LoginResponseDto>>
{
    public async Task<Result<LoginResponseDto>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByVerifyTokenAsync(request.Token, cancellationToken);
        if (account is null)
            return Result.Failure<LoginResponseDto>(Error.NotFound("Account", request.Token));

        var verifyResult = account.VerifyEmail(request.Token);
        if (verifyResult.IsFailure) return Result.Failure<LoginResponseDto>(verifyResult.Error);

        var jti = Guid.NewGuid().ToString();
        var accessToken = jwtService.GenerateAccessToken(account.Id, account.Email, account.Role.ToString(), jti);
        var (refreshRaw, refreshHash) = jwtService.GenerateRefreshToken();
        var refreshToken = DomainRefreshToken.Create(account.Id, refreshHash, jti, null, null);

        await refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        accountRepository.Update(account);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var tokens = new AuthTokensDto(accessToken, refreshRaw, 900);
        var user = new AuthUserDto(account.Id, account.Email, string.Empty, account.Role.ToString(), "Free", false, 1);
        return Result.Success(new LoginResponseDto(tokens, user));
    }
}
