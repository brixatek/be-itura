using Itura.Auth.Application.Common.Interfaces;
using Itura.Auth.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;
using System.Security.Cryptography;
using System.Text;

namespace Itura.Auth.Application.Features.Auth.ResetPassword;

internal sealed class ResetPasswordCommandHandler(
    IAccountRepository accountRepository,
    IPasswordResetTokenRepository resetTokenRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IAuthUnitOfWork unitOfWork,
    IEmailService emailService,
    IPasswordHasher passwordHasher) : IRequestHandler<ResetPasswordCommand, Result>
{
    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(request.Token)));
        var resetToken = await resetTokenRepository.GetByTokenHashAsync(tokenHash, cancellationToken);

        if (resetToken is null || !resetToken.IsValid)
            return Result.Failure(Error.Validation("Auth.InvalidToken", "Invalid or expired reset token."));

        var account = await accountRepository.GetByIdAsync(resetToken.AccountId, cancellationToken);
        if (account is null)
            return Result.Failure(Error.NotFound("Account", resetToken.AccountId));

        var newPasswordHash = passwordHasher.Hash(request.NewPassword);
        account.ChangePassword(newPasswordHash);
        resetToken.MarkUsed();

        await refreshTokenRepository.RevokeAllForAccountAsync(account.Id, cancellationToken);

        accountRepository.Update(account);
        resetTokenRepository.Update(resetToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await emailService.SendPasswordChangedNotificationAsync(account.Email, string.Empty, cancellationToken);

        return Result.Success();
    }
}
