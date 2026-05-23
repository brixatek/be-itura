using Itura.Community.Domain.Enums;
using Itura.SharedKernel.Domain;

namespace Itura.Community.Domain.Events;

public sealed record CommunityPostCreatedDomainEvent(
    Guid PostId,
    Guid AuthorUserId,
    PostType PostType,
    string? Title) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
