using Itura.Community.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Community.Application.Features.Posts.Queries.GetHotFeed;

public sealed record HotPostDto(
    Guid Id,
    string? Title,
    string Body,
    string PostType,
    Guid AuthorUserId,
    List<string> Tags,
    int LikeCount,
    int CommentCount,
    bool IsPublic,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    double HotScore);

public sealed record GetHotFeedQuery(
    int Page = 1,
    int PageSize = 20,
    int WindowDays = 7) : IRequest<Result<IReadOnlyList<HotPostDto>>>;
