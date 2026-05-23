using Itura.Contracts.Media;
using Itura.Media.Domain.Events;
using MassTransit;
using MediatR;

namespace Itura.Media.Infrastructure.EventHandlers;

internal sealed class MediaAssetUploadedDomainEventHandler(IPublishEndpoint publishEndpoint)
    : INotificationHandler<MediaAssetUploadedDomainEvent>
{
    public async Task Handle(MediaAssetUploadedDomainEvent notification, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(new MediaAssetUploadedEvent(
            notification.MediaAssetId,
            notification.Url,
            notification.MediaType.ToString(),
            notification.MimeType,
            notification.UploaderUserId,
            notification.OccurredAt), cancellationToken);
    }
}
