using Itura.Payment.Domain.Entities;
using Itura.SharedKernel.Results;

namespace Itura.Payment.Domain.Repositories;

public interface IWalletRepository
{
    Task<Wallet?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<Wallet?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Wallet wallet, CancellationToken ct = default);
    void Update(Wallet wallet);
    Task AddTransactionAsync(WalletTransaction transaction, CancellationToken ct = default);
    Task<PagedResult<WalletTransaction>> GetTransactionsAsync(Guid userId, int page, int pageSize, CancellationToken ct = default);
}
