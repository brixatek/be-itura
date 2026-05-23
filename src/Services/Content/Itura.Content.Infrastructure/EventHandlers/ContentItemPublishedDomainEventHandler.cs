using Itura.Contracts.Content;
using Itura.Content.Domain.Events;
using MassTransit;
using MediatR;

namespace Itura.Content.Infrastructure.EventHandlers;

internal sealed class ContentItemPublishedDomainEventHandler(IPublishEndpoint publishEndpoint)
    : INotificationHandler<ContentItemPublishedDomainEvent>
{
    public async Task Handle(ContentItemPublishedDomainEvent notification, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(new ContentPublishedEvent(
            notification.ContentItemId,
            notification.Title,
            notification.Slug,
            notification.ContentType.ToString(),
            notification.AuthorUserId,
            notification.Tags,
            notification.ThumbnailUrl,
            notification.PublishedAt), cancellationToken);
    }
}
