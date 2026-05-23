using Itura.Auth.Domain.Entities;
using Itura.Auth.Domain.Repositories;
using Itura.Auth.Infrastructure.Persistence;

namespace Itura.Auth.Infrastructure.Repositories;

public sealed class AuthAuditLogRepository(AuthDbContext db) : IAuthAuditLogRepository
{
    public async Task AddAsync(AuthAuditLog log, CancellationToken ct = default)
        => await db.AuthAuditLogs.AddAsync(log, ct);
}
