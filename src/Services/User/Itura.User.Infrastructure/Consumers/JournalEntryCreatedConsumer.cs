using Itura.Contracts.Journal;
using Itura.User.Application.Features.Gamification;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Itura.User.Infrastructure.Consumers;

public sealed class JournalEntryCreatedConsumer(ISender sender, ILogger<JournalEntryCreatedConsumer> logger)
    : IConsumer<JournalEntryCreatedEvent>
{
    public async Task Consume(ConsumeContext<JournalEntryCreatedEvent> context)
    {
        var msg = context.Message;
        var ct = context.CancellationToken;

        // Award XP for journaling
        var xpResult = await sender.Send(new AwardXpCommand(msg.UserId, 10, "journal.entry", msg.EntryId), ct);
        if (xpResult.IsFailure)
            logger.LogWarning("Failed to award XP for journal entry {EntryId}: {Error}", msg.EntryId, xpResult.Error);

        // Record journaling streak
        var streakResult = await sender.Send(
            new RecordStreakActivityCommand(msg.UserId, "journal", DateOnly.FromDateTime(msg.CreatedAt)), ct);
        if (streakResult.IsFailure)
            logger.LogWarning("Failed to record streak for journal entry {EntryId}: {Error}", msg.EntryId, streakResult.Error);
    }
}
