using Itura.Auth.Domain.Entities;
using Itura.Auth.Domain.Repositories;
using Itura.Auth.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Itura.Auth.Infrastructure.Repositories;

internal sealed class PasswordResetTokenRepository(AuthDbContext context) : IPasswordResetTokenRepository
{
    public Task<PasswordResetToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default) =>
        context.PasswordResetTokens.FirstOrDefaultAsync(t => t.TokenHash == tokenHash, ct);

    public async Task AddAsync(PasswordResetToken token, CancellationToken ct = default) =>
        await context.PasswordResetTokens.AddAsync(token, ct);

    public void Update(PasswordResetToken token) =>
        context.PasswordResetTokens.Update(token);

    public async Task InvalidateAllForAccountAsync(Guid accountId, CancellationToken ct = default)
    {
        var active = await context.PasswordResetTokens
            .Where(t => t.AccountId == accountId && t.UsedAt == null && t.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(ct);

        foreach (var t in active)
            t.MarkUsed();
    }
}
