using Itura.Auth.Application.Common.Interfaces;
using Itura.Auth.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;
using System.Security.Cryptography;

namespace Itura.Auth.Application.Features.Auth.VerifyEmail;

internal sealed class ResendVerificationCommandHandler(
    IAccountRepository accountRepository,
    IAuthUnitOfWork unitOfWork,
    ICacheService cache,
    IEmailService emailService)
    : IRequestHandler<ResendVerificationCommand, Result>
{
    public async Task<Result> Handle(ResendVerificationCommand request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByEmailAsync(request.Email.ToLowerInvariant(), cancellationToken);
        if (account is null)
            return Result.Success(); // don't leak whether email exists

        if (account.EmailVerified)
            return Result.Success(); // already verified — silently succeed

        var rateLimitKey = $"email_verify_resend:{account.Id}";
        var countStr = await cache.GetAsync(rateLimitKey, cancellationToken);
        var count = int.TryParse(countStr, out var c) ? c : 0;

        if (count >= 3)
            return Result.Failure(Error.Validation("Auth.ResendLimitExceeded",
                "Maximum resend attempts (3) reached. Please wait 1 hour before trying again."));

        var newToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .Replace("+", "-").Replace("/", "_").TrimEnd('=');

        var refreshResult = account.RefreshVerifyToken(newToken);
        if (refreshResult.IsFailure) return refreshResult;

        accountRepository.Update(account);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Increment rate limit counter; expire in 1 hour
        await cache.SetAsync(rateLimitKey, (count + 1).ToString(), TimeSpan.FromHours(1), cancellationToken);

        await emailService.SendEmailVerificationAsync(request.Email, string.Empty, newToken, cancellationToken);

        return Result.Success();
    }
}
