using Itura.Auth.Application.Common.Interfaces;
using Itura.Auth.Domain.Entities;
using Itura.Auth.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Auth.Application.Features.Auth.Register;

internal sealed class RegisterCommandHandler(
    IAccountRepository accountRepository,
    IAuthUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IEmailService emailService) : IRequestHandler<RegisterCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await accountRepository.ExistsByEmailAsync(request.Email, cancellationToken))
            return Result.Failure<Guid>(Error.Conflict("Auth.EmailTaken", "An account with this email already exists."));

        var passwordHash = passwordHasher.Hash(request.Password);
        var verifyToken = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32))
            .Replace("+", "-").Replace("/", "_").TrimEnd('=');

        var result = Account.Create(request.Email, passwordHash, verifyToken, request.FullName, request.Timezone);
        if (result.IsFailure) return Result.Failure<Guid>(result.Error);

        await accountRepository.AddAsync(result.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await emailService.SendEmailVerificationAsync(request.Email, request.FullName, verifyToken, cancellationToken);

        return Result.Success(result.Value.Id);
    }
}
