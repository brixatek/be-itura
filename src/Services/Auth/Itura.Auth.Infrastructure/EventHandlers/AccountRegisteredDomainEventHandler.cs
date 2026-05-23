using Itura.Auth.Domain.Events;
using Itura.Contracts.Auth;
using MassTransit;
using MediatR;

namespace Itura.Auth.Infrastructure.EventHandlers;

internal sealed class AccountRegisteredDomainEventHandler(IPublishEndpoint publishEndpoint)
    : INotificationHandler<AccountRegisteredDomainEvent>
{
    public async Task Handle(AccountRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(new UserRegisteredEvent(
            notification.AccountId,
            notification.Email,
            notification.FullName,
            notification.Timezone,
            notification.OccurredAt), cancellationToken);
    }
}
