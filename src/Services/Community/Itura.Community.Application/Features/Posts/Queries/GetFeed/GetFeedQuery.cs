using Itura.Community.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Community.Application.Features.Posts.Queries.GetFeed;

public sealed record GetFeedQuery(
    string? Cursor,
    int PageSize = 20,
    string? PostType = null,
    string? Tag = null) : IRequest<Result<CursorPagedResult<CommunityPostDto>>>;
