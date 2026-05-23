using Itura.Payment.Domain.Enums;
using Itura.Payment.Domain.Events;
using Itura.SharedKernel.Domain;
using Itura.SharedKernel.Results;

namespace Itura.Payment.Domain.Entities;

public sealed class Wallet : AggregateRoot
{
    public Guid UserId { get; private set; }
    public decimal Balance { get; private set; }
    public int SessionCredits { get; private set; }
    public string Currency { get; private set; } = "NGN";
    public int Version { get; private set; }

    private Wallet() { }

    public static Wallet Create(Guid userId, string currency = "NGN")
    {
        return new Wallet
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Balance = 0m,
            SessionCredits = 0,
            Currency = currency.ToUpperInvariant(),
            Version = 0
        };
    }

    public Result Credit(decimal amount, string description)
    {
        if (amount <= 0)
            return Result.Failure(Error.Validation("Wallet.Credit", "Credit amount must be positive."));

        Balance += amount;
        Version++;
        MarkUpdated();
        return Result.Success();
    }

    public Result Debit(decimal amount, string description, int expectedVersion)
    {
        if (amount <= 0)
            return Result.Failure(Error.Validation("Wallet.Debit", "Debit amount must be positive."));

        if (Version != expectedVersion)
            return Result.Failure(Error.Conflict("Wallet.ConcurrencyConflict", "Wallet was modified concurrently. Please retry."));

        if (Balance < amount)
            return Result.Failure(Error.Validation("Wallet.InsufficientFunds", "Insufficient wallet balance."));

        Balance -= amount;
        Version++;
        MarkUpdated();
        return Result.Success();
    }

    public void AddSessionCredits(int credits)
    {
        SessionCredits += credits;
        MarkUpdated();
    }

    public Result UseSessionCredit()
    {
        if (SessionCredits <= 0)
            return Result.Failure(Error.Validation("Wallet.NoCredits", "No session credits available."));

        SessionCredits--;
        MarkUpdated();
        return Result.Success();
    }
}
