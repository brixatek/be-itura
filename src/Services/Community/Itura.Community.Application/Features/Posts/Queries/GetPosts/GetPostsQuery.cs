using Itura.Community.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Community.Application.Features.Posts.Queries.GetPosts;

public sealed record GetPostsQuery(
    int Page,
    int PageSize,
    string? PostType,
    string? Tag,
    Guid? AuthorUserId) : IRequest<Result<PagedResult<CommunityPostDto>>>;
