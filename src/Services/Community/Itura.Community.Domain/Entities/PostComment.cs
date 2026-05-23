using Itura.SharedKernel.Domain;
using Itura.SharedKernel.Results;

namespace Itura.Community.Domain.Entities;

public sealed class PostComment : AggregateRoot
{
    public Guid PostId { get; private set; }
    public Guid AuthorUserId { get; private set; }
    public string Body { get; private set; } = string.Empty;
    public int LikeCount { get; private set; }

    private PostComment() { }

    public static Result<PostComment> Create(Guid postId, Guid authorUserId, string body)
    {
        if (postId == Guid.Empty)
            return Result.Failure<PostComment>(Error.Validation("PostComment.PostId", "Post is required."));

        if (authorUserId == Guid.Empty)
            return Result.Failure<PostComment>(Error.Validation("PostComment.AuthorUserId", "Author is required."));

        if (string.IsNullOrWhiteSpace(body))
            return Result.Failure<PostComment>(Error.Validation("PostComment.Body", "Body is required."));

        return new PostComment
        {
            PostId = postId,
            AuthorUserId = authorUserId,
            Body = body.Trim()
        };
    }

    public void Like() => LikeCount++;
    public void Delete() => MarkDeleted();
}
