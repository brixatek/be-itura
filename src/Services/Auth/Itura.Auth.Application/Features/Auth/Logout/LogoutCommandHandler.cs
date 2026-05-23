using Itura.Auth.Application.Common.Interfaces;
using Itura.Auth.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;
using System.Security.Cryptography;
using System.Text;

namespace Itura.Auth.Application.Features.Auth.Logout;

internal sealed class LogoutCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IAuthUnitOfWork unitOfWork,
    ICacheService cacheService) : IRequestHandler<LogoutCommand, Result>
{
    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        // Blacklist the current access token
        await cacheService.BlacklistJtiAsync(request.Jti, TimeSpan.FromMinutes(16), cancellationToken);

        if (request.LogoutAllDevices)
        {
            await refreshTokenRepository.RevokeAllForAccountAsync(request.AccountId, cancellationToken);
        }
        else
        {
            var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(request.RefreshToken)));
            var token = await refreshTokenRepository.GetByTokenHashAsync(tokenHash, cancellationToken);
            if (token is not null && token.IsActive)
            {
                token.Revoke();
                refreshTokenRepository.Update(token);
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
