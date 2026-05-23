using Itura.Analytics.Application.Common.Interfaces;
using Itura.Analytics.Domain.Entities;
using Itura.Analytics.Domain.Repositories;
using Itura.Contracts.Mood;
using MassTransit;
using System.Text.Json;

namespace Itura.Analytics.Infrastructure.Consumers;

internal sealed class MoodLoggedConsumer(
    IAnalyticsEventRepository repository,
    IAnalyticsUnitOfWork unitOfWork) : IConsumer<MoodLoggedEvent>
{
    public async Task Consume(ConsumeContext<MoodLoggedEvent> context)
    {
        var e = context.Message;
        var props = JsonSerializer.Serialize(new
        {
            e.EntryId, e.Score, e.Emotions, e.LoggedAt
        });
        var @event = AnalyticsEvent.Create(e.UserId, "mood.logged", "mood", props);
        await repository.AddAsync(@event, context.CancellationToken);
        await unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}
