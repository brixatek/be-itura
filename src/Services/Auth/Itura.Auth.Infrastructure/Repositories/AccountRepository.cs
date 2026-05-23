using Itura.Auth.Domain.Entities;
using Itura.Auth.Domain.Repositories;
using Itura.Auth.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Itura.Auth.Infrastructure.Repositories;

internal sealed class AccountRepository(AuthDbContext context) : IAccountRepository
{
    public Task<Account?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        context.Accounts.FirstOrDefaultAsync(a => a.Id == id, ct);

    public Task<Account?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        context.Accounts.FirstOrDefaultAsync(a => a.Email == email.ToLowerInvariant(), ct);

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default) =>
        context.Accounts.AnyAsync(a => a.Email == email.ToLowerInvariant(), ct);

    public Task<Account?> GetByVerifyTokenAsync(string token, CancellationToken ct = default) =>
        context.Accounts.FirstOrDefaultAsync(a => a.EmailVerifyToken == token, ct);

    public async Task AddAsync(Account account, CancellationToken ct = default) =>
        await context.Accounts.AddAsync(account, ct);

    public void Update(Account account) =>
        context.Accounts.Update(account);
}
