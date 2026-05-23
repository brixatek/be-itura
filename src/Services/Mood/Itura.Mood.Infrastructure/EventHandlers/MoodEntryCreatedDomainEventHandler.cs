using Itura.Contracts.Mood;
using Itura.Mood.Domain.Events;
using MassTransit;
using MediatR;

namespace Itura.Mood.Infrastructure.EventHandlers;

internal sealed class MoodEntryCreatedDomainEventHandler(IPublishEndpoint publishEndpoint)
    : INotificationHandler<MoodEntryCreatedDomainEvent>
{
    public async Task Handle(MoodEntryCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(new MoodLoggedEvent(
            notification.EntryId,
            notification.UserId,
            notification.Score,
            notification.Emotions,
            notification.LoggedAt), cancellationToken);
    }
}
