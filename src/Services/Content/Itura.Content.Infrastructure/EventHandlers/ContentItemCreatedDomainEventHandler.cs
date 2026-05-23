using Itura.Content.Domain.Events;
using MediatR;

namespace Itura.Content.Infrastructure.EventHandlers;

internal sealed class ContentItemCreatedDomainEventHandler
    : INotificationHandler<ContentItemCreatedDomainEvent>
{
    public Task Handle(ContentItemCreatedDomainEvent notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
