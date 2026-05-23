using Itura.Booking.Domain.Events;
using Itura.Contracts.Booking;
using MassTransit;
using MediatR;

namespace Itura.Booking.Infrastructure.EventHandlers;

internal sealed class BookingCancelledDomainEventHandler(IPublishEndpoint publishEndpoint)
    : INotificationHandler<BookingCancelledDomainEvent>
{
    public async Task Handle(BookingCancelledDomainEvent notification, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(new BookingCancelledEvent(
            notification.BookingId,
            notification.CoachId,
            notification.ClientUserId,
            notification.Reason,
            notification.OccurredAt), cancellationToken);
    }
}
