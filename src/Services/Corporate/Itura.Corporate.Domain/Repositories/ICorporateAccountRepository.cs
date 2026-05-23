using Itura.Corporate.Domain.Entities;

namespace Itura.Corporate.Domain.Repositories;

public interface ICorporateAccountRepository
{
    Task<CorporateAccount?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<CorporateAccount?> GetByAdminUserIdAsync(Guid adminUserId, CancellationToken ct = default);
    Task AddAsync(CorporateAccount account, CancellationToken ct = default);
    void Update(CorporateAccount account);
}
