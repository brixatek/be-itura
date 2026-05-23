namespace Itura.Contracts.Journal;

public sealed record JournalEntryCreatedEvent(
    Guid EntryId,
    Guid UserId,
    string Title,
    IReadOnlyList<string> Tags,
    DateTime CreatedAt);
