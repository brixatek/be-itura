using Itura.Contracts.User;
using Itura.User.Domain.Events;
using MassTransit;
using MediatR;

namespace Itura.User.Infrastructure.EventHandlers;

internal sealed class PreferencesUpdatedDomainEventHandler(IPublishEndpoint publishEndpoint)
    : INotificationHandler<PreferencesUpdatedDomainEvent>
{
    public async Task Handle(PreferencesUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(new PreferencesUpdatedEvent(
            notification.AccountId,
            notification.EmailNotifications,
            notification.PushNotifications,
            notification.WeeklyDigest,
            notification.Theme,
            notification.Language,
            notification.OccurredAt), cancellationToken);
    }
}
