using Itura.SharedKernel.Domain;

namespace Itura.Journal.Domain.Events;

public sealed record JournalEntryCreatedDomainEvent(
    Guid EntryId,
    Guid UserId,
    string Title,
    IReadOnlyList<string> Tags,
    DateTime CreatedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
