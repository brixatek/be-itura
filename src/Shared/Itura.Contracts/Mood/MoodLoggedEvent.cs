namespace Itura.Contracts.Mood;

public sealed record MoodLoggedEvent(
    Guid EntryId,
    Guid UserId,
    int Score,
    IReadOnlyList<string> Emotions,
    DateTime LoggedAt);
