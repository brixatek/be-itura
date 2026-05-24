using Itura.AI.Application.Common.Interfaces;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.AI.Application.Features.AI.Journaling;

internal sealed class GetJournalPromptsQueryHandler(
    IAICompletionService aiService,
    IJournalPromptsCache cache)
    : IRequestHandler<GetJournalPromptsQuery, Result<List<string>>>
{
    private const string SystemPrompt =
        "You are a wellness journaling coach. Generate 3 diverse, personalized, open-ended journal prompts. " +
        "Keep each prompt to 1-2 sentences. Return exactly 3 prompts, one per line, numbered 1. 2. 3.";

    public async Task<Result<List<string>>> Handle(GetJournalPromptsQuery request, CancellationToken cancellationToken)
    {
        var cached = await cache.GetAsync(request.UserId, cancellationToken);
        if (cached is not null)
            return Result.Success(cached);

        var raw = await aiService.CompleteAsync(SystemPrompt, [],
            "Generate 3 diverse journaling prompts for a wellness app user.", cancellationToken);

        var prompts = ParsePrompts(raw);
        await cache.SetAsync(request.UserId, prompts, cancellationToken);

        return Result.Success(prompts);
    }

    private static List<string> ParsePrompts(string raw)
    {
        var lines = raw.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var prompts = lines
            .Where(l => l.TrimStart().StartsWith("1.") || l.TrimStart().StartsWith("2.") || l.TrimStart().StartsWith("3."))
            .Select(l => System.Text.RegularExpressions.Regex.Replace(l.Trim(), @"^\d+\.\s*", ""))
            .Take(3)
            .ToList();

        return prompts.Count == 3 ? prompts :
        [
            "What made you feel most alive today, and why?",
            "Describe a recent challenge and what you learned from it.",
            "What are three things you're grateful for right now?"
        ];
    }
}
