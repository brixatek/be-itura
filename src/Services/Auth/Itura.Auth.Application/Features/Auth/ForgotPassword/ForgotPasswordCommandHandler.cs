using Itura.Auth.Application.Common.Interfaces;
using Itura.Auth.Domain.Entities;
using Itura.Auth.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;
using System.Security.Cryptography;
using System.Text;

namespace Itura.Auth.Application.Features.Auth.ForgotPassword;

internal sealed class ForgotPasswordCommandHandler(
    IAccountRepository accountRepository,
    IPasswordResetTokenRepository resetTokenRepository,
    IAuthUnitOfWork unitOfWork,
    IEmailService emailService) : IRequestHandler<ForgotPasswordCommand, Result>
{
    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByEmailAsync(request.Email, cancellationToken);

        // Always return success to prevent email enumeration
        if (account is null) return Result.Success();

        await resetTokenRepository.InvalidateAllForAccountAsync(account.Id, cancellationToken);

        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .Replace("+", "-").Replace("/", "_").TrimEnd('=');
        var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken)));

        var resetToken = PasswordResetToken.Create(account.Id, tokenHash);
        await resetTokenRepository.AddAsync(resetToken, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await emailService.SendPasswordResetAsync(account.Email, string.Empty, rawToken, cancellationToken);

        return Result.Success();
    }
}
