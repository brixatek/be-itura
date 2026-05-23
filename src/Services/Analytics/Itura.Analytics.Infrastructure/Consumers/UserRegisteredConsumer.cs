using Itura.Analytics.Application.Common.Interfaces;
using Itura.Analytics.Domain.Entities;
using Itura.Analytics.Domain.Repositories;
using Itura.Contracts.Auth;
using MassTransit;
using System.Text.Json;

namespace Itura.Analytics.Infrastructure.Consumers;

internal sealed class UserRegisteredConsumer(
    IAnalyticsEventRepository repository,
    IAnalyticsUnitOfWork unitOfWork) : IConsumer<UserRegisteredEvent>
{
    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        var e = context.Message;
        var props = JsonSerializer.Serialize(new { e.Email, e.FullName, e.Timezone });
        var @event = AnalyticsEvent.Create(e.AccountId, "user.registered", "auth", props);
        await repository.AddAsync(@event, context.CancellationToken);
        await unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}
