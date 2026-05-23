namespace Itura.Contracts.Community;

public sealed record CommunityPostCreatedEvent(
    Guid PostId,
    Guid AuthorUserId,
    string PostType,
    string? Title,
    DateTime CreatedAt);
