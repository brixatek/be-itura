using Itura.Auth.Domain.Events;
using Itura.Contracts.Auth;
using MassTransit;
using MediatR;

namespace Itura.Auth.Infrastructure.EventHandlers;

internal sealed class AccountMarkedForDeletionDomainEventHandler(IPublishEndpoint publishEndpoint)
    : INotificationHandler<AccountMarkedForDeletionDomainEvent>
{
    public async Task Handle(AccountMarkedForDeletionDomainEvent notification, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(new AccountDeletedEvent(
            notification.AccountId,
            notification.Email,
            notification.OccurredAt), cancellationToken);
    }
}
