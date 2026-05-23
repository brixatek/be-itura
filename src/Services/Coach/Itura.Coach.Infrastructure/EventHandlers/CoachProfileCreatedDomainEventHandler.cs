using Itura.Coach.Domain.Events;
using Itura.Contracts.Coach;
using MassTransit;
using MediatR;

namespace Itura.Coach.Infrastructure.EventHandlers;

internal sealed class CoachProfileCreatedDomainEventHandler(IPublishEndpoint publishEndpoint)
    : INotificationHandler<CoachProfileCreatedDomainEvent>
{
    public async Task Handle(CoachProfileCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(new CoachProfileCreatedEvent(
            notification.CoachId,
            notification.UserId,
            notification.DisplayName,
            notification.Specializations,
            notification.Languages,
            notification.HourlyRate,
            notification.Currency,
            notification.CreatedAt), cancellationToken);
    }
}
