using Itura.Booking.Domain.Events;
using Itura.Contracts.Booking;
using MassTransit;
using MediatR;

namespace Itura.Booking.Infrastructure.EventHandlers;

internal sealed class BookingCompletedDomainEventHandler(IPublishEndpoint publishEndpoint)
    : INotificationHandler<BookingCompletedDomainEvent>
{
    public async Task Handle(BookingCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(new BookingCompletedEvent(
            notification.BookingId,
            notification.CoachId,
            notification.ClientUserId,
            notification.Price,
            notification.Currency,
            notification.OccurredAt), cancellationToken);
    }
}
