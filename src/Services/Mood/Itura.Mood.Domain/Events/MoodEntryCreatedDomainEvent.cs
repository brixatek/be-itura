using Itura.SharedKernel.Domain;

namespace Itura.Mood.Domain.Events;

public sealed record MoodEntryCreatedDomainEvent(
    Guid EntryId,
    Guid UserId,
    int Score,
    IReadOnlyList<string> Emotions,
    DateTime LoggedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
