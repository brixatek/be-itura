using Itura.Contracts.Journal;
using Itura.Journal.Domain.Events;
using MassTransit;
using MediatR;

namespace Itura.Journal.Infrastructure.EventHandlers;

internal sealed class JournalEntryCreatedDomainEventHandler(IPublishEndpoint publishEndpoint)
    : INotificationHandler<JournalEntryCreatedDomainEvent>
{
    public async Task Handle(JournalEntryCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(new JournalEntryCreatedEvent(
            notification.EntryId,
            notification.UserId,
            notification.Title,
            notification.Tags,
            notification.CreatedAt), cancellationToken);
    }
}
