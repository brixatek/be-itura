namespace Itura.Contracts.Content;

public sealed record ContentPublishedEvent(
    Guid ContentItemId,
    string Title,
    string Slug,
    string ContentType,
    Guid AuthorUserId,
    List<string> Tags,
    string? ThumbnailUrl,
    DateTime PublishedAt);
