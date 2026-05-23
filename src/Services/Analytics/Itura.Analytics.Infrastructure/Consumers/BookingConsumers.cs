using Itura.Analytics.Application.Common.Interfaces;
using Itura.Analytics.Domain.Entities;
using Itura.Analytics.Domain.Repositories;
using Itura.Contracts.Booking;
using MassTransit;
using System.Text.Json;

namespace Itura.Analytics.Infrastructure.Consumers;

internal sealed class BookingCreatedConsumer(
    IAnalyticsEventRepository repository,
    IAnalyticsUnitOfWork unitOfWork) : IConsumer<BookingCreatedEvent>
{
    public async Task Consume(ConsumeContext<BookingCreatedEvent> context)
    {
        var e = context.Message;
        var props = JsonSerializer.Serialize(new
        {
            e.BookingId, e.CoachId, e.CoachUserId,
            e.DurationMinutes, e.Price, e.Currency, e.ScheduledAt
        });
        var @event = AnalyticsEvent.Create(e.ClientUserId, "booking.created", "booking", props);
        await repository.AddAsync(@event, context.CancellationToken);
        await unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}

internal sealed class BookingCompletedConsumer(
    IAnalyticsEventRepository repository,
    IAnalyticsUnitOfWork unitOfWork) : IConsumer<BookingCompletedEvent>
{
    public async Task Consume(ConsumeContext<BookingCompletedEvent> context)
    {
        var e = context.Message;
        var props = JsonSerializer.Serialize(new
        {
            e.BookingId, e.CoachId, e.Price, e.Currency, e.CompletedAt
        });
        var @event = AnalyticsEvent.Create(e.ClientUserId, "booking.completed", "booking", props);
        await repository.AddAsync(@event, context.CancellationToken);
        await unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}

internal sealed class BookingCancelledConsumer(
    IAnalyticsEventRepository repository,
    IAnalyticsUnitOfWork unitOfWork) : IConsumer<BookingCancelledEvent>
{
    public async Task Consume(ConsumeContext<BookingCancelledEvent> context)
    {
        var e = context.Message;
        var props = JsonSerializer.Serialize(new
        {
            e.BookingId, e.CoachId, e.CancellationReason, e.CancelledAt
        });
        var @event = AnalyticsEvent.Create(e.ClientUserId, "booking.cancelled", "booking", props);
        await repository.AddAsync(@event, context.CancellationToken);
        await unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}
