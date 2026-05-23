using Itura.Analytics.Application.Common.Interfaces;
using Itura.Analytics.Domain.Entities;
using Itura.Analytics.Domain.Repositories;
using Itura.Contracts.Payment;
using MassTransit;
using System.Text.Json;

namespace Itura.Analytics.Infrastructure.Consumers;

internal sealed class PaymentCompletedConsumer(
    IAnalyticsEventRepository repository,
    IAnalyticsUnitOfWork unitOfWork) : IConsumer<PaymentCompletedEvent>
{
    public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
    {
        var e = context.Message;
        var props = JsonSerializer.Serialize(new
        {
            e.PaymentId, e.BookingId, e.PayeeUserId,
            e.Amount, e.Currency, e.TransactionReference, e.CompletedAt
        });
        var @event = AnalyticsEvent.Create(e.PayerUserId, "payment.completed", "payment", props);
        await repository.AddAsync(@event, context.CancellationToken);
        await unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}
