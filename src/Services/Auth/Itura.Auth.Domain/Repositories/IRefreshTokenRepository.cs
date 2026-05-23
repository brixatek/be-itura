using Itura.Auth.Domain.Entities;

namespace Itura.Auth.Domain.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default);
    Task AddAsync(RefreshToken token, CancellationToken ct = default);
    void Update(RefreshToken token);
    Task RevokeAllForAccountAsync(Guid accountId, CancellationToken ct = default);
}
