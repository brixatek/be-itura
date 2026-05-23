using Itura.Auth.Application.Common.Interfaces;
using Itura.Auth.Application.DTOs;
using Itura.Auth.Domain.Entities;
using Itura.Auth.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;
using System.Security.Cryptography;
using System.Text;

namespace Itura.Auth.Application.Features.Auth.RefreshToken;

internal sealed class RefreshTokenCommandHandler(
    IAccountRepository accountRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IAuthUnitOfWork unitOfWork,
    IJwtService jwtService,
    ICacheService cacheService) : IRequestHandler<RefreshTokenCommand, Result<AuthTokensDto>>
{
    public async Task<Result<AuthTokensDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(request.RefreshToken)));
        var storedToken = await refreshTokenRepository.GetByTokenHashAsync(tokenHash, cancellationToken);

        if (storedToken is null || !storedToken.IsActive)
            return Result.Failure<AuthTokensDto>(Error.Unauthorized("Invalid or expired refresh token."));

        var account = await accountRepository.GetByIdAsync(storedToken.AccountId, cancellationToken);
        if (account is null || !account.CanLogin())
            return Result.Failure<AuthTokensDto>(Error.Unauthorized("Account is inactive."));

        // Rotate: revoke old, issue new
        var newJti = Guid.NewGuid().ToString();
        var newAccessToken = jwtService.GenerateAccessToken(account.Id, account.Email, account.Role.ToString(), newJti);
        var (newRefreshRaw, newRefreshHash) = jwtService.GenerateRefreshToken();

        var newRefreshToken = Domain.Entities.RefreshToken.Create(
            account.Id, newRefreshHash, newJti, request.DeviceInfo, request.IpAddress);

        storedToken.Revoke(newRefreshToken.Id);

        // Blacklist old access token jti if still within its lifetime
        await cacheService.BlacklistJtiAsync(storedToken.Jti, TimeSpan.FromMinutes(16), cancellationToken);

        await refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);
        refreshTokenRepository.Update(storedToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new AuthTokensDto(newAccessToken, newRefreshRaw, 900));
    }
}
