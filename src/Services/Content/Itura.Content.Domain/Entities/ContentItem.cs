using Itura.Content.Domain.Enums;
using Itura.Content.Domain.Events;
using Itura.SharedKernel.Domain;
using Itura.SharedKernel.Results;
using System.Text.RegularExpressions;

namespace Itura.Content.Domain.Entities;

public sealed class ContentItem : AggregateRoot
{
    public string Title { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Body { get; private set; }
    public ContentType ContentType { get; private set; }
    public Guid AuthorUserId { get; private set; }
    public List<string> Tags { get; private set; } = [];
    public bool IsPublished { get; private set; }
    public DateTime? PublishedAt { get; private set; }
    public string? ThumbnailUrl { get; private set; }
    public string? MediaUrl { get; private set; }
    public int? DurationSeconds { get; private set; }
    public int ViewCount { get; private set; }

    private ContentItem() { }

    public static Result<ContentItem> Create(
        string title,
        string? description,
        string? body,
        ContentType contentType,
        Guid authorUserId,
        List<string>? tags = null,
        string? thumbnailUrl = null,
        string? mediaUrl = null,
        int? durationSeconds = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<ContentItem>(Error.Validation("ContentItem.Title", "Title is required."));

        if (authorUserId == Guid.Empty)
            return Result.Failure<ContentItem>(Error.Validation("ContentItem.AuthorUserId", "Author is required."));

        var item = new ContentItem
        {
            Title = title.Trim(),
            Slug = GenerateSlug(title),
            Description = description,
            Body = body,
            ContentType = contentType,
            AuthorUserId = authorUserId,
            Tags = tags ?? [],
            ThumbnailUrl = thumbnailUrl,
            MediaUrl = mediaUrl,
            DurationSeconds = durationSeconds,
            IsPublished = false
        };

        item.RaiseDomainEvent(new ContentItemCreatedDomainEvent(item.Id, item.Title, item.Slug, item.ContentType, item.AuthorUserId));
        return item;
    }

    public Result Publish()
    {
        if (IsPublished)
            return Result.Failure(Error.Validation("ContentItem.Publish", "Content is already published."));

        IsPublished = true;
        PublishedAt = DateTime.UtcNow;

        RaiseDomainEvent(new ContentItemPublishedDomainEvent(
            Id, Title, Slug, ContentType, AuthorUserId, [.. Tags], ThumbnailUrl, PublishedAt.Value));

        return Result.Success();
    }

    public Result Unpublish()
    {
        if (!IsPublished)
            return Result.Failure(Error.Validation("ContentItem.Unpublish", "Content is not published."));

        IsPublished = false;
        return Result.Success();
    }

    public Result Update(
        string title,
        string? description,
        string? body,
        List<string>? tags,
        string? thumbnailUrl,
        string? mediaUrl,
        int? durationSeconds)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure(Error.Validation("ContentItem.Title", "Title is required."));

        Title = title.Trim();
        Slug = GenerateSlug(title);
        Description = description;
        Body = body;
        Tags = tags ?? [];
        ThumbnailUrl = thumbnailUrl;
        MediaUrl = mediaUrl;
        DurationSeconds = durationSeconds;
        return Result.Success();
    }

    public void IncrementViewCount() => ViewCount++;

    public void Delete() => MarkDeleted();

    private static string GenerateSlug(string title)
    {
        var slug = title.ToLowerInvariant().Trim();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-").Trim('-');
        return $"{slug}-{Guid.NewGuid().ToString("N")[..8]}";
    }
}
