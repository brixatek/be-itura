using Itura.Community.Domain.Enums;
using Itura.Community.Domain.Events;
using Itura.SharedKernel.Domain;
using Itura.SharedKernel.Results;

namespace Itura.Community.Domain.Entities;

public sealed class CommunityPost : AggregateRoot
{
    public string? Title { get; private set; }
    public string Body { get; private set; } = string.Empty;
    public PostType PostType { get; private set; }
    public Guid AuthorUserId { get; private set; }
    public List<string> Tags { get; private set; } = [];
    public int LikeCount { get; private set; }
    public int CommentCount { get; private set; }
    public int ReactionCount { get; private set; }
    public bool IsPublic { get; private set; }
    public bool IsAnonymous { get; private set; }

    private CommunityPost() { }

    public static Result<CommunityPost> Create(
        string body,
        PostType postType,
        Guid authorUserId,
        string? title = null,
        List<string>? tags = null,
        bool isPublic = true,
        bool isAnonymous = false)
    {
        if (string.IsNullOrWhiteSpace(body))
            return Result.Failure<CommunityPost>(Error.Validation("CommunityPost.Body", "Body is required."));

        if (authorUserId == Guid.Empty)
            return Result.Failure<CommunityPost>(Error.Validation("CommunityPost.AuthorUserId", "Author is required."));

        var post = new CommunityPost
        {
            Title = title?.Trim(),
            Body = body.Trim(),
            PostType = postType,
            AuthorUserId = authorUserId,
            Tags = tags ?? [],
            IsPublic = isPublic,
            IsAnonymous = isAnonymous
        };

        post.RaiseDomainEvent(new CommunityPostCreatedDomainEvent(
            post.Id, post.AuthorUserId, post.PostType, post.Title));

        return post;
    }

    public Result Update(string body, string? title, List<string>? tags)
    {
        if (string.IsNullOrWhiteSpace(body))
            return Result.Failure(Error.Validation("CommunityPost.Body", "Body is required."));

        Title = title?.Trim();
        Body = body.Trim();
        Tags = tags ?? [];
        return Result.Success();
    }

    public void Like() => LikeCount++;
    public void Unlike() => LikeCount = Math.Max(0, LikeCount - 1);
    public void IncrementComments() => CommentCount++;
    public void DecrementComments() => CommentCount = Math.Max(0, CommentCount - 1);
    public void IncrementReactions() => ReactionCount++;
    public void DecrementReactions() => ReactionCount = Math.Max(0, ReactionCount - 1);
    public void Delete() => MarkDeleted();
}
