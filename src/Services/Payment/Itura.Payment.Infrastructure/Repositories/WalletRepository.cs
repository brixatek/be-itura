using Itura.Payment.Domain.Entities;
using Itura.Payment.Domain.Repositories;
using Itura.Payment.Infrastructure.Persistence;
using Itura.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace Itura.Payment.Infrastructure.Repositories;

internal sealed class WalletRepository(PaymentDbContext context) : IWalletRepository
{
    public async Task<Wallet?> GetByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        await context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId, ct);

    public async Task<Wallet?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.Wallets.FirstOrDefaultAsync(w => w.Id == id, ct);

    public async Task AddAsync(Wallet wallet, CancellationToken ct = default) =>
        await context.Wallets.AddAsync(wallet, ct);

    public void Update(Wallet wallet) =>
        context.Wallets.Update(wallet);

    public async Task AddTransactionAsync(WalletTransaction transaction, CancellationToken ct = default) =>
        await context.WalletTransactions.AddAsync(transaction, ct);

    public async Task<PagedResult<WalletTransaction>> GetTransactionsAsync(
        Guid userId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = context.WalletTransactions.Where(t => t.UserId == userId);
        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        return new PagedResult<WalletTransaction>(items, total, page, pageSize);
    }
}
