namespace Itura.Community.Application.DTOs;

public sealed record PostCommentDto(
    Guid Id,
    Guid PostId,
    Guid AuthorUserId,
    string Body,
    int LikeCount,
    DateTime CreatedAt);
