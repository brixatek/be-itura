using Itura.AI.Domain.Entities;

namespace Itura.AI.Domain.Repositories;

public interface IConversationRepository
{
    Task<Conversation?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<List<Conversation>> GetByUserIdAsync(Guid userId, int limit = 20, CancellationToken ct = default);
    Task UpsertAsync(Conversation conversation, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
}
