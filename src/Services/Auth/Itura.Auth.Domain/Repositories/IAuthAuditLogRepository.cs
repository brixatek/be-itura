using Itura.Auth.Domain.Entities;

namespace Itura.Auth.Domain.Repositories;

public interface IAuthAuditLogRepository
{
    Task AddAsync(AuthAuditLog log, CancellationToken ct = default);
}
