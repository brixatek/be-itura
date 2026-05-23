using Itura.Contracts.Corporate;
using Itura.Corporate.Domain.Events;
using MassTransit;
using MediatR;

namespace Itura.Corporate.Infrastructure.EventHandlers;

internal sealed class CorporateAccountCreatedDomainEventHandler(IPublishEndpoint publishEndpoint)
    : INotificationHandler<CorporateAccountCreatedDomainEvent>
{
    public async Task Handle(CorporateAccountCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(new CorporateAccountCreatedEvent(
            notification.CorporateAccountId,
            notification.CompanyName,
            notification.AdminUserId,
            notification.SubscriptionTier.ToString(),
            notification.OccurredAt), cancellationToken);
    }
}
