namespace Itura.Content.Application.DTOs;

public sealed record ContentItemDto(
    Guid Id,
    string Title,
    string Slug,
    string? Description,
    string? Body,
    string ContentType,
    Guid AuthorUserId,
    List<string> Tags,
    bool IsPublished,
    DateTime? PublishedAt,
    string? ThumbnailUrl,
    string? MediaUrl,
    int? DurationSeconds,
    int ViewCount,
    DateTime CreatedAt,
    DateTime UpdatedAt);
