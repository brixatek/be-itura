using Itura.SharedKernel.Entities;

namespace Itura.Payment.Domain.Entities;

public sealed class WalletTransaction : AuditableEntity
{
    public Guid WalletId { get; private set; }
    public Guid UserId { get; private set; }
    public decimal Amount { get; private set; }
    public string Type { get; private set; } = string.Empty; // credit, debit
    public string Description { get; private set; } = string.Empty;
    public string? Reference { get; private set; }
    public decimal BalanceAfter { get; private set; }

    private WalletTransaction() { }

    public static WalletTransaction Create(
        Guid walletId, Guid userId, decimal amount,
        string type, string description, decimal balanceAfter, string? reference = null)
    {
        return new WalletTransaction
        {
            Id = Guid.NewGuid(),
            WalletId = walletId,
            UserId = userId,
            Amount = amount,
            Type = type,
            Description = description,
            Reference = reference,
            BalanceAfter = balanceAfter
        };
    }
}
