using Itura.Auth.Domain.Entities;

namespace Itura.Auth.Domain.Repositories;

public interface IPasswordResetTokenRepository
{
    Task<PasswordResetToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default);
    Task AddAsync(PasswordResetToken token, CancellationToken ct = default);
    void Update(PasswordResetToken token);
    Task InvalidateAllForAccountAsync(Guid accountId, CancellationToken ct = default);
}
