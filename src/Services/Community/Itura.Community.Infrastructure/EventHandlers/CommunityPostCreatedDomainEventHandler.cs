using Itura.Contracts.Community;
using Itura.Community.Domain.Events;
using MassTransit;
using MediatR;

namespace Itura.Community.Infrastructure.EventHandlers;

internal sealed class CommunityPostCreatedDomainEventHandler(IPublishEndpoint publishEndpoint)
    : INotificationHandler<CommunityPostCreatedDomainEvent>
{
    public async Task Handle(CommunityPostCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(new CommunityPostCreatedEvent(
            notification.PostId,
            notification.AuthorUserId,
            notification.PostType.ToString(),
            notification.Title,
            notification.OccurredAt), cancellationToken);
    }
}
