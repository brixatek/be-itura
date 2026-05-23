using Itura.Corporate.Domain.Entities;
using Itura.Corporate.Domain.Repositories;
using Itura.Corporate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Itura.Corporate.Infrastructure.Repositories;

internal sealed class CorporateAccountRepository(CorporateDbContext context) : ICorporateAccountRepository
{
    public async Task<CorporateAccount?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.Accounts.FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<CorporateAccount?> GetByAdminUserIdAsync(Guid adminUserId, CancellationToken ct = default) =>
        await context.Accounts.FirstOrDefaultAsync(e => e.AdminUserId == adminUserId, ct);

    public async Task AddAsync(CorporateAccount account, CancellationToken ct = default) =>
        await context.Accounts.AddAsync(account, ct);

    public void Update(CorporateAccount account) => context.Accounts.Update(account);
}
