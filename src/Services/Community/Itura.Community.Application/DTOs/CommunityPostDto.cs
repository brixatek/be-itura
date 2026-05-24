namespace Itura.Community.Application.DTOs;

public sealed record CommunityPostDto(
    Guid Id,
    string? Title,
    string Body,
    string PostType,
    Guid? AuthorUserId,
    List<string> Tags,
    int LikeCount,
    int CommentCount,
    bool IsPublic,
    bool IsAnonymous,
    DateTime CreatedAt,
    DateTime UpdatedAt);
