using Itura.AI.Domain.Entities;
using Itura.AI.Domain.Repositories;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Itura.AI.Infrastructure.Repositories;

internal sealed class ConversationRepository(
    IMongoDatabase database,
    ILogger<ConversationRepository> logger) : IConversationRepository
{
    private IMongoCollection<Conversation> Collection =>
        database.GetCollection<Conversation>("conversations");

    public async Task<Conversation?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        try
        {
            return await Collection.Find(c => c.Id == id).FirstOrDefaultAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to get conversation {Id}", id);
            return null;
        }
    }

    public async Task<List<Conversation>> GetByUserIdAsync(Guid userId, int limit = 20, CancellationToken ct = default)
    {
        try
        {
            return await Collection
                .Find(c => c.UserId == userId)
                .SortByDescending(c => c.UpdatedAt)
                .Limit(limit)
                .ToListAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to list conversations for user {UserId}", userId);
            return [];
        }
    }

    public async Task UpsertAsync(Conversation conversation, CancellationToken ct = default)
    {
        try
        {
            var options = new ReplaceOptions { IsUpsert = true };
            await Collection.ReplaceOneAsync(c => c.Id == conversation.Id, conversation, options, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to upsert conversation {Id}", conversation.Id);
        }
    }

    public async Task DeleteAsync(string id, CancellationToken ct = default)
    {
        try
        {
            await Collection.DeleteOneAsync(c => c.Id == id, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to delete conversation {Id}", id);
        }
    }
}
