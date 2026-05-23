using Itura.Auth.Domain.Enums;
using Itura.Auth.Domain.Events;
using Itura.SharedKernel.Domain;
using Itura.SharedKernel.Results;

namespace Itura.Auth.Domain.Entities;

public sealed class Account : AggregateRoot
{
    private const int MaxFailedLoginAttempts = 5;
    private const int LockoutMinutes = 30;

    private Account() { } // EF Core

    public string Email { get; private set; } = string.Empty;
    public string? PasswordHash { get; private set; }
    public bool EmailVerified { get; private set; }
    public string? EmailVerifyToken { get; private set; }
    public DateTime? EmailVerifyExpiry { get; private set; }
    public AuthProvider AuthProvider { get; private set; } = AuthProvider.Local;
    public string? OAuthProviderId { get; private set; }
    public UserRole Role { get; private set; } = UserRole.User;
    public AccountStatus Status { get; private set; } = AccountStatus.Active;
    public int FailedLoginCount { get; private set; }
    public DateTime? LockoutUntil { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    public ICollection<RefreshToken> RefreshTokens { get; private set; } = [];
    public ICollection<PasswordResetToken> PasswordResetTokens { get; private set; } = [];

    public static Result<Account> Create(string email, string passwordHash, string emailVerifyToken, string fullName, string timezone)
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Email = email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            EmailVerifyToken = emailVerifyToken,
            EmailVerifyExpiry = DateTime.UtcNow.AddHours(24),
            AuthProvider = AuthProvider.Local,
            Status = AccountStatus.Active,
        };

        account.RaiseDomainEvent(new AccountRegisteredDomainEvent(account.Id, account.Email, fullName, timezone));
        return account;
    }

    public static Account CreateOAuth(string email, AuthProvider provider, string providerId)
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Email = email.ToLowerInvariant(),
            EmailVerified = true,
            AuthProvider = provider,
            OAuthProviderId = providerId,
            Status = AccountStatus.Active,
        };
        return account;
    }

    public Result VerifyEmail(string token)
    {
        if (EmailVerified)
            return Result.Failure(Error.Conflict("Auth.AlreadyVerified", "Email is already verified."));

        if (EmailVerifyToken != token)
            return Result.Failure(Error.Validation("Auth.InvalidToken", "Invalid verification token."));

        if (EmailVerifyExpiry < DateTime.UtcNow)
            return Result.Failure(Error.Validation("Auth.TokenExpired", "Verification token has expired."));

        EmailVerified = true;
        EmailVerifyToken = null;
        EmailVerifyExpiry = null;
        MarkUpdated();

        RaiseDomainEvent(new AccountEmailVerifiedDomainEvent(Id, Email));
        return Result.Success();
    }

    public Result RecordSuccessfulLogin()
    {
        if (Status == AccountStatus.Suspended)
            return Result.Failure(Error.Unauthorized("Account is suspended."));

        if (IsLockedOut())
            return Result.Failure(Error.Unauthorized($"Account is locked. Try again after {LockoutUntil:u}."));

        FailedLoginCount = 0;
        LockoutUntil = null;
        LastLoginAt = DateTime.UtcNow;
        MarkUpdated();
        return Result.Success();
    }

    public void RecordFailedLogin()
    {
        FailedLoginCount++;

        if (FailedLoginCount >= MaxFailedLoginAttempts)
            LockoutUntil = DateTime.UtcNow.AddMinutes(LockoutMinutes);

        MarkUpdated();
    }

    public Result ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        MarkUpdated();
        return Result.Success();
    }

    public void SetPasswordResetToken(PasswordResetToken token) =>
        PasswordResetTokens.Add(token);

    public bool IsLockedOut() =>
        LockoutUntil.HasValue && LockoutUntil.Value > DateTime.UtcNow;

    public bool CanLogin() =>
        Status == AccountStatus.Active && !IsLockedOut();
}
