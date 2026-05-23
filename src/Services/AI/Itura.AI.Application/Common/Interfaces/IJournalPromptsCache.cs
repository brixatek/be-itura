namespace Itura.AI.Application.Common.Interfaces;

public interface IJournalPromptsCache
{
    Task<List<string>?> GetAsync(Guid userId, CancellationToken ct = default);
    Task SetAsync(Guid userId, List<string> prompts, CancellationToken ct = default);
}
