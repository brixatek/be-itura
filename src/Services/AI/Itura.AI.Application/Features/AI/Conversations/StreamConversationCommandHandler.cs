using Itura.AI.Application.Common.Interfaces;
using Itura.AI.Domain.Entities;
using Itura.AI.Domain.Repositories;
using Itura.Contracts.AI;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Text;

namespace Itura.AI.Application.Features.AI.Conversations;

internal sealed class StreamConversationCommandHandler(
    IConversationRepository conversations,
    IAICompletionService aiService,
    IAIRateLimiter rateLimiter,
    IPublishEndpoint publishEndpoint,
    ILogger<StreamConversationCommandHandler> logger)
    : IStreamRequestHandler<StreamConversationCommand, string>
{
    private static readonly string[] CrisisKeywords =
    [
        "suicide", "kill myself", "end my life", "want to die", "self harm",
        "cut myself", "hurt myself", "don't want to live", "no reason to live"
    ];

    private static readonly string SystemPrompt =
        "You are Sera, a compassionate AI wellness companion for Itura. " +
        "You provide empathetic, non-clinical support for mental wellness. " +
        "You are warm, supportive, and non-judgmental. " +
        "You are NOT a therapist — always encourage professional help when appropriate. " +
        "Keep responses concise (2-3 paragraphs max). " +
        "If the user expresses crisis or self-harm intent, immediately acknowledge their pain and provide crisis resources.";

    private static readonly string CrisisOverrideResponse =
        "I hear you, and I'm really concerned about what you've shared. " +
        "Please reach out to a crisis helpline right now — you're not alone. " +
        "In Nigeria: Suicide Research and Prevention Initiative: 0800-100-2000. " +
        "You can also text a trusted person or go to your nearest emergency room. " +
        "Your life has value, and support is available.";

    private static int GetDailyLimit(string tier) => tier.ToLowerInvariant() switch
    {
        "premium" => 50,
        "pro" or "unlimited" => int.MaxValue,
        _ => 5
    };

    public async IAsyncEnumerable<string> Handle(
        StreamConversationCommand request,
        [EnumeratorCancellation] CancellationToken ct)
    {
        var dailyLimit = GetDailyLimit(request.Tier);
        var rateStatus = await rateLimiter.CheckAsync(request.UserId, dailyLimit, ct);
        if (!rateStatus.IsAllowed)
        {
            yield return $"[ERROR:RateLimit] Daily message limit reached. Resets at {rateStatus.ResetAt:HH:mm} UTC.";
            yield break;
        }

        Conversation conversation;
        if (!string.IsNullOrEmpty(request.ConversationId))
        {
            conversation = await conversations.GetByIdAsync(request.ConversationId, ct)
                ?? new Conversation { UserId = request.UserId };
        }
        else
        {
            conversation = new Conversation { UserId = request.UserId };
        }

        var crisisDetected = request.Message
            .ToLowerInvariant()
            .Let(lower => CrisisKeywords.Any(k => lower.Contains(k)));

        conversation.AddMessage("user", request.Message);

        if (crisisDetected)
        {
            logger.LogWarning("Crisis keywords detected for user {UserId} during stream", request.UserId);

            var snippet = request.Message.Length > 100
                ? request.Message[..100] + "..."
                : request.Message;

            await publishEndpoint.Publish(new CrisisDetectedEvent(
                request.UserId, snippet, "AI", DateTime.UtcNow), ct);

            foreach (var word in CrisisOverrideResponse.Split(' '))
            {
                yield return word + " ";
            }

            conversation.AddMessage("assistant", CrisisOverrideResponse);
            await conversations.UpsertAsync(conversation, ct);
            yield break;
        }

        var history = conversation.Messages
            .SkipLast(1)
            .TakeLast(10)
            .Select(m => (m.Role, m.Content));

        var replyBuilder = new StringBuilder();

        await foreach (var token in aiService.StreamCompletionAsync(SystemPrompt, history, request.Message, ct))
        {
            replyBuilder.Append(token);
            yield return token;
        }

        conversation.AddMessage("assistant", replyBuilder.ToString());
        await conversations.UpsertAsync(conversation, ct);
    }
}

file static class StringExtensions
{
    public static TResult Let<T, TResult>(this T value, Func<T, TResult> block) => block(value);
}
