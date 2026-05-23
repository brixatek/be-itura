using Itura.User.Domain.Entities;

namespace Itura.User.Domain.Repositories;

public interface IXpRepository
{
    Task AddTransactionAsync(XpTransaction transaction, CancellationToken ct = default);
    Task<List<XpTransaction>> GetByUserIdAsync(Guid userProfileId, int page, int pageSize, CancellationToken ct = default);
    Task<List<(Guid UserId, int TotalXp)>> GetTopUsersAsync(int count, CancellationToken ct = default);
}
