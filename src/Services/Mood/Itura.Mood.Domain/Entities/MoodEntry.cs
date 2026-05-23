using Itura.Mood.Domain.Events;
using Itura.SharedKernel.Domain;
using Itura.SharedKernel.Results;

namespace Itura.Mood.Domain.Entities;

public sealed class MoodEntry : AggregateRoot
{
    public Guid UserId { get; private set; }
    public int Score { get; private set; }
    public string? Note { get; private set; }
    public List<string> Emotions { get; private set; } = [];
    public DateTime LoggedAt { get; private set; }

    private MoodEntry() { }

    public static Result<MoodEntry> Create(
        Guid userId, int score, string? note, IEnumerable<string> emotions, DateTime? loggedAt = null)
    {
        if (score < 1 || score > 10)
            return Result.Failure<MoodEntry>(new Error("MoodEntry.InvalidScore", "Score must be between 1 and 10."));

        var entry = new MoodEntry
        {
            UserId = userId,
            Score = score,
            Note = note?.Trim(),
            Emotions = emotions.Select(e => e.Trim().ToLowerInvariant()).Distinct().ToList(),
            LoggedAt = loggedAt ?? DateTime.UtcNow
        };

        entry.RaiseDomainEvent(new MoodEntryCreatedDomainEvent(entry.Id, userId, score, entry.Emotions, entry.LoggedAt));

        return Result.Success(entry);
    }

    public void Delete() => MarkDeleted();
}
