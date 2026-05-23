using Itura.User.Domain.Entities;
using Itura.User.Domain.Repositories;
using Itura.User.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Itura.User.Infrastructure.Repositories;

internal sealed class XpRepository(UserDbContext context) : IXpRepository
{
    public async Task AddTransactionAsync(XpTransaction transaction, CancellationToken ct = default) =>
        await context.XpTransactions.AddAsync(transaction, ct);

    public Task<List<XpTransaction>> GetByUserIdAsync(Guid userProfileId, int page, int pageSize, CancellationToken ct = default) =>
        context.XpTransactions
            .Where(x => x.UserProfileId == userProfileId)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public Task<List<(Guid UserId, int TotalXp)>> GetTopUsersAsync(int count, CancellationToken ct = default) =>
        context.UserProfiles
            .OrderByDescending(p => p.TotalXp)
            .Take(count)
            .Select(p => new { p.Id, p.TotalXp })
            .ToListAsync(ct)
            .ContinueWith(t => t.Result.Select(x => (x.Id, x.TotalXp)).ToList(), ct);
}
