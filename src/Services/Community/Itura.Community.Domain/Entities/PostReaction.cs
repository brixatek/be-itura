using Itura.SharedKernel.Entities;

namespace Itura.Community.Domain.Entities;

public sealed class PostReaction : AuditableEntity
{
    public Guid PostId { get; private set; }
    public Guid UserId { get; private set; }
    public string Emoji { get; private set; } = string.Empty;

    private PostReaction() { }

    public static PostReaction Create(Guid postId, Guid userId, string emoji) =>
        new()
        {
            Id = Guid.NewGuid(),
            PostId = postId,
            UserId = userId,
            Emoji = emoji
        };
}
