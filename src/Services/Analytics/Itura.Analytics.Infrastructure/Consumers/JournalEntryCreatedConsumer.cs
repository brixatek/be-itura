using Itura.Analytics.Application.Common.Interfaces;
using Itura.Analytics.Domain.Entities;
using Itura.Analytics.Domain.Repositories;
using Itura.Contracts.Journal;
using MassTransit;
using System.Text.Json;

namespace Itura.Analytics.Infrastructure.Consumers;

internal sealed class JournalEntryCreatedConsumer(
    IAnalyticsEventRepository repository,
    IAnalyticsUnitOfWork unitOfWork) : IConsumer<JournalEntryCreatedEvent>
{
    public async Task Consume(ConsumeContext<JournalEntryCreatedEvent> context)
    {
        var e = context.Message;
        var props = JsonSerializer.Serialize(new
        {
            e.EntryId, e.Title, e.Tags, e.CreatedAt
        });
        var @event = AnalyticsEvent.Create(e.UserId, "journal.entry.created", "journal", props);
        await repository.AddAsync(@event, context.CancellationToken);
        await unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}
