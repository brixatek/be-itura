using Itura.Auth.Domain.Entities;
using Itura.Auth.Domain.Enums;
using Itura.Auth.Domain.Repositories;
using Itura.Auth.Infrastructure.Persistence;
using Itura.SharedKernel.Results;
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

    public async Task<PagedResult<Account>> SearchAsync(
        string? search, AccountStatus? status, UserRole? role,
        DateTime? registeredFrom, DateTime? registeredTo,
        int page, int pageSize, CancellationToken ct = default)
    {
        var query = context.Accounts.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(a => a.Email.Contains(search.ToLowerInvariant()));

        if (status.HasValue)
            query = query.Where(a => a.Status == status.Value);

        if (role.HasValue)
            query = query.Where(a => a.Role == role.Value);

        if (registeredFrom.HasValue)
            query = query.Where(a => a.CreatedAt >= registeredFrom.Value);

        if (registeredTo.HasValue)
            query = query.Where(a => a.CreatedAt <= registeredTo.Value);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<Account>(items, total, page, pageSize);
    }

    public Task<int> CountAsync(AccountStatus? status, CancellationToken ct = default)
    {
        var query = context.Accounts.AsQueryable();
        if (status.HasValue)
            query = query.Where(a => a.Status == status.Value);
        return query.CountAsync(ct);
    }

    public async Task AddAsync(Account account, CancellationToken ct = default) =>
        await context.Accounts.AddAsync(account, ct);

    public void Update(Account account) =>
        context.Accounts.Update(account);
}
