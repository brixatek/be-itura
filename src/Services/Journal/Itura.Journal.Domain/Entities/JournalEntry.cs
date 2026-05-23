using Itura.Journal.Domain.Events;
using Itura.SharedKernel.Domain;
using Itura.SharedKernel.Results;

namespace Itura.Journal.Domain.Entities;

public sealed class JournalEntry : AggregateRoot
{
    public Guid UserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public List<string> Tags { get; private set; } = [];
    public int? MoodScore { get; private set; }
    public bool IsPrivate { get; private set; }

    private JournalEntry() { }

    public static Result<JournalEntry> Create(
        Guid userId, string title, string content,
        IEnumerable<string>? tags = null, int? moodScore = null, bool isPrivate = false)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<JournalEntry>(new Error("JournalEntry.EmptyTitle", "Title cannot be empty."));

        if (moodScore.HasValue && (moodScore < 1 || moodScore > 10))
            return Result.Failure<JournalEntry>(new Error("JournalEntry.InvalidMoodScore", "Mood score must be between 1 and 10."));

        var entry = new JournalEntry
        {
            UserId = userId,
            Title = title.Trim(),
            Content = content?.Trim() ?? string.Empty,
            Tags = (tags ?? []).Select(t => t.Trim().ToLowerInvariant()).Distinct().Where(t => t.Length > 0).ToList(),
            MoodScore = moodScore,
            IsPrivate = isPrivate
        };

        entry.RaiseDomainEvent(new JournalEntryCreatedDomainEvent(
            entry.Id, userId, entry.Title, entry.Tags, entry.CreatedAt));

        return Result.Success(entry);
    }

    public Result Update(string title, string content, IEnumerable<string>? tags, int? moodScore, bool isPrivate)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure(new Error("JournalEntry.EmptyTitle", "Title cannot be empty."));

        if (moodScore.HasValue && (moodScore < 1 || moodScore > 10))
            return Result.Failure(new Error("JournalEntry.InvalidMoodScore", "Mood score must be between 1 and 10."));

        Title = title.Trim();
        Content = content?.Trim() ?? string.Empty;
        Tags = (tags ?? []).Select(t => t.Trim().ToLowerInvariant()).Distinct().Where(t => t.Length > 0).ToList();
        MoodScore = moodScore;
        IsPrivate = isPrivate;
        MarkUpdated();

        return Result.Success();
    }

    public void Delete() => MarkDeleted();
}
