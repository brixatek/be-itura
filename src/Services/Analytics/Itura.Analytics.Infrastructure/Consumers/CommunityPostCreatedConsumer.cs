using Itura.Analytics.Application.Common.Interfaces;
using Itura.Analytics.Domain.Entities;
using Itura.Analytics.Domain.Repositories;
using Itura.Contracts.Community;
using MassTransit;
using System.Text.Json;

namespace Itura.Analytics.Infrastructure.Consumers;

internal sealed class CommunityPostCreatedConsumer(
    IAnalyticsEventRepository repository,
    IAnalyticsUnitOfWork unitOfWork) : IConsumer<CommunityPostCreatedEvent>
{
    public async Task Consume(ConsumeContext<CommunityPostCreatedEvent> context)
    {
        var e = context.Message;
        var props = JsonSerializer.Serialize(new
        {
            e.PostId, e.PostType, e.Title, e.CreatedAt
        });
        var @event = AnalyticsEvent.Create(e.AuthorUserId, "community.post.created", "community", props);
        await repository.AddAsync(@event, context.CancellationToken);
        await unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}
