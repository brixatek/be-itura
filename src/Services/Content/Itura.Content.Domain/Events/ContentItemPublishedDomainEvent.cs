using Itura.Content.Domain.Enums;
using Itura.SharedKernel.Domain;

namespace Itura.Content.Domain.Events;

public sealed record ContentItemPublishedDomainEvent(
    Guid ContentItemId,
    string Title,
    string Slug,
    ContentType ContentType,
    Guid AuthorUserId,
    List<string> Tags,
    string? ThumbnailUrl,
    DateTime PublishedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
