using Itura.Community.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Community.Application.Features.Posts.Queries.GetHotFeed;

internal sealed class GetHotFeedQueryHandler(ICommunityPostRepository repository)
    : IRequestHandler<GetHotFeedQuery, Result<IReadOnlyList<HotPostDto>>>
{
    public async Task<Result<IReadOnlyList<HotPostDto>>> Handle(
        GetHotFeedQuery request, CancellationToken cancellationToken)
    {
        var windowDays = Math.Clamp(request.WindowDays, 1, 30);
        var since = DateTime.UtcNow.AddDays(-windowDays);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var posts = await repository.GetRecentAsync(since, cancellationToken);

        var now = DateTime.UtcNow;
        var sorted = posts
            .Select(p =>
            {
                var ageHours = (now - p.CreatedAt).TotalHours;
                var score = (p.LikeCount * 2.0 + p.CommentCount) / Math.Pow(ageHours + 2, 1.5);
                return new HotPostDto(
                    p.Id, p.Title, p.Body, p.PostType.ToString(),
                    p.AuthorUserId, p.Tags, p.LikeCount, p.CommentCount,
                    p.IsPublic, p.CreatedAt, p.UpdatedAt, Math.Round(score, 6));
            })
            .OrderByDescending(p => p.HotScore)
            .Skip((request.Page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Result.Success<IReadOnlyList<HotPostDto>>(sorted);
    }
}
