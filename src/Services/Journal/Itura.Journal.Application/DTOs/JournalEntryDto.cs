namespace Itura.Journal.Application.DTOs;

public sealed record JournalEntryDto(
    Guid Id,
    Guid UserId,
    string Title,
    string Content,
    IReadOnlyList<string> Tags,
    int? MoodScore,
    bool IsPrivate,
    DateTime CreatedAt,
    DateTime UpdatedAt);
