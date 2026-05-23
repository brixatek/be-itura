using Itura.Contracts.Gamification;
using Itura.Gamification.Domain.Events;
using MassTransit;
using MediatR;

namespace Itura.Gamification.Infrastructure.EventHandlers;

internal sealed class PointsAwardedDomainEventHandler(IPublishEndpoint publishEndpoint)
    : INotificationHandler<PointsAwardedDomainEvent>
{
    public async Task Handle(PointsAwardedDomainEvent notification, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(new PointsAwardedEvent(
            notification.UserId,
            notification.Points,
            notification.NewTotal,
            notification.NewLevel,
            notification.Reason,
            notification.OccurredAt), cancellationToken);
    }
}
