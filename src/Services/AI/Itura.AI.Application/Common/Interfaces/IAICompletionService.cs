namespace Itura.AI.Application.Common.Interfaces;

public interface IAICompletionService
{
    IAsyncEnumerable<string> StreamCompletionAsync(
        string systemPrompt,
        IEnumerable<(string Role, string Content)> history,
        string userMessage,
        CancellationToken ct = default);

    Task<string> CompleteAsync(
        string systemPrompt,
        IEnumerable<(string Role, string Content)> history,
        string userMessage,
        CancellationToken ct = default);
}
