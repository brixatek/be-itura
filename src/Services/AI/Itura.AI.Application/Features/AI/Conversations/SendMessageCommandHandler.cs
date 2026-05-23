using Itura.AI.Application.Common.Interfaces;
using Itura.AI.Domain.Entities;
using Itura.AI.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Itura.AI.Application.Features.AI.Conversations;

internal sealed class SendMessageCommandHandler(
    IConversationRepository conversations,
    IAICompletionService aiService,
    IAIRateLimiter rateLimiter,
    ILogger<SendMessageCommandHandler> logger)
    : IRequestHandler<SendMessageCommand, Result<SendMessageResult>>
{
    private const int DefaultDailyLimit = 50;

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

    public async Task<Result<SendMessageResult>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var rateStatus = await rateLimiter.CheckAsync(request.UserId, DefaultDailyLimit, cancellationToken);
        if (!rateStatus.IsAllowed)
            return Result.Failure<SendMessageResult>(
                Error.Validation("AI.RateLimited", $"Daily message limit reached. Resets at {rateStatus.ResetAt:HH:mm} UTC."));

        Conversation conversation;
        if (!string.IsNullOrEmpty(request.ConversationId))
        {
            conversation = await conversations.GetByIdAsync(request.ConversationId, cancellationToken)
                ?? new Conversation { UserId = request.UserId };
        }
        else
        {
            conversation = new Conversation { UserId = request.UserId };
        }

        var crisisDetected = DetectCrisis(request.Message);
        conversation.AddMessage("user", request.Message);

        string reply;
        if (crisisDetected)
        {
            logger.LogWarning("Crisis keywords detected for user {UserId} in conversation {ConversationId}",
                request.UserId, conversation.Id);
            reply = CrisisOverrideResponse;
        }
        else
        {
            var history = conversation.Messages
                .SkipLast(1) // exclude the message we just added
                .TakeLast(10) // last 10 turns for context window management
                .Select(m => (m.Role, m.Content));

            reply = await aiService.CompleteAsync(SystemPrompt, request.Message, cancellationToken);
        }

        conversation.AddMessage("assistant", reply);
        await conversations.UpsertAsync(conversation, cancellationToken);

        return Result.Success(new SendMessageResult(conversation.Id, reply, crisisDetected));
    }

    private static bool DetectCrisis(string message)
    {
        var lower = message.ToLowerInvariant();
        return CrisisKeywords.Any(k => lower.Contains(k));
    }
}
