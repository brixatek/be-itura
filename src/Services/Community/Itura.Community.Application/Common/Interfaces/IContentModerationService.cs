namespace Itura.Community.Application.Common.Interfaces;

public sealed record ModerationResult(bool IsFlagged, string? Reason);

public interface IContentModerationService
{
    Task<ModerationResult> CheckAsync(string content, CancellationToken ct = default);
}
