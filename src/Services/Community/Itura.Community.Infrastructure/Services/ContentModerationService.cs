using Itura.Community.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Itura.Community.Infrastructure.Services;

internal sealed class ContentModerationService(ILogger<ContentModerationService> logger) : IContentModerationService
{
    private static readonly string[] BlockedTerms =
    [
        "spam", "scam", "buy now", "click here", "free money",
        "hate", "violence", "harassment", "abuse"
    ];

    public Task<ModerationResult> CheckAsync(string content, CancellationToken ct = default)
    {
        var lower = content.ToLowerInvariant();

        foreach (var term in BlockedTerms)
        {
            if (lower.Contains(term))
            {
                logger.LogWarning("Content moderation flagged post containing term: {Term}", term);
                return Task.FromResult(new ModerationResult(true, $"Content contains prohibited term: '{term}'."));
            }
        }

        return Task.FromResult(new ModerationResult(false, null));
    }
}
