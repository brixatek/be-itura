using Itura.Auth.Domain.Entities;
using Itura.Auth.Domain.Repositories;
using Itura.Auth.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Itura.Auth.Infrastructure.Repositories;

internal sealed class RefreshTokenRepository(AuthDbContext context) : IRefreshTokenRepository
{
    public Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default) =>
        context.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == tokenHash, ct);

    public async Task AddAsync(RefreshToken token, CancellationToken ct = default) =>
        await context.RefreshTokens.AddAsync(token, ct);

    public void Update(RefreshToken token) =>
        context.RefreshTokens.Update(token);

    public async Task RevokeAllForAccountAsync(Guid accountId, CancellationToken ct = default)
    {
        var activeTokens = await context.RefreshTokens
            .Where(t => t.AccountId == accountId && t.RevokedAt == null && t.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(ct);

        foreach (var token in activeTokens)
            token.Revoke();
    }
}
