using Itura.Community.Application.DTOs;
using Itura.Community.Application.Features.Posts.Queries.GetPost;
using Itura.Community.Domain.Enums;
using Itura.Community.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;
using System.Text;

namespace Itura.Community.Application.Features.Posts.Queries.GetFeed;

internal sealed class GetFeedQueryHandler(ICommunityPostRepository repository)
    : IRequestHandler<GetFeedQuery, Result<CursorPagedResult<CommunityPostDto>>>
{
    public async Task<Result<CursorPagedResult<CommunityPostDto>>> Handle(
        GetFeedQuery request, CancellationToken cancellationToken)
    {
        PostType? postType = null;
        if (!string.IsNullOrWhiteSpace(request.PostType))
        {
            if (!Enum.TryParse<PostType>(request.PostType, true, out var parsed))
                return Result.Failure<CursorPagedResult<CommunityPostDto>>(
                    Error.Validation("Post.PostType", "Invalid post type."));
            postType = parsed;
        }

        (Guid? cursorId, DateTime? cursorCreatedAt) = DecodeCursor(request.Cursor);

        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var posts = await repository.GetFeedAsync(
            cursorId, cursorCreatedAt, pageSize + 1, postType, request.Tag, cancellationToken);

        var hasMore = posts.Count > pageSize;
        if (hasMore) posts = posts[..pageSize];

        string? nextCursor = hasMore ? EncodeCursor(posts[^1].CreatedAt, posts[^1].Id) : null;

        var dtos = posts.Select(GetPostQueryHandler.ToDto).ToList();
        return new CursorPagedResult<CommunityPostDto>(dtos, nextCursor, hasMore);
    }

    private static string EncodeCursor(DateTime createdAt, Guid id)
    {
        var raw = $"{createdAt:O}|{id:N}";
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(raw));
    }

    private static (Guid? Id, DateTime? CreatedAt) DecodeCursor(string? cursor)
    {
        if (string.IsNullOrWhiteSpace(cursor)) return (null, null);
        try
        {
            var raw = Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
            var parts = raw.Split('|');
            if (parts.Length != 2) return (null, null);
            if (!DateTime.TryParse(parts[0], null,
                    System.Globalization.DateTimeStyles.RoundtripKind, out var dt)) return (null, null);
            if (!Guid.TryParseExact(parts[1], "N", out var id)) return (null, null);
            return (id, dt);
        }
        catch
        {
            return (null, null);
        }
    }
}
