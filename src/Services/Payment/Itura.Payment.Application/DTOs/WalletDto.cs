namespace Itura.Payment.Application.DTOs;

public sealed record WalletDto(
    Guid Id,
    Guid UserId,
    decimal Balance,
    int SessionCredits,
    string Currency);

public sealed record WalletTransactionDto(
    Guid Id,
    decimal Amount,
    string Type,
    string Description,
    string? Reference,
    decimal BalanceAfter,
    DateTime CreatedAt);
