using Itura.Community.Application.DTOs;
using Itura.Community.Application.Features.Posts.Queries.GetPost;
using Itura.Community.Domain.Enums;
using Itura.Community.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Community.Application.Features.Posts.Queries.GetPosts;

internal sealed class GetPostsQueryHandler(ICommunityPostRepository repository)
    : IRequestHandler<GetPostsQuery, Result<PagedResult<CommunityPostDto>>>
{
    public async Task<Result<PagedResult<CommunityPostDto>>> Handle(GetPostsQuery request, CancellationToken cancellationToken)
    {
        PostType? postType = null;
        if (!string.IsNullOrWhiteSpace(request.PostType))
        {
            if (!Enum.TryParse<PostType>(request.PostType, true, out var parsed))
                return Result.Failure<PagedResult<CommunityPostDto>>(
                    Error.Validation("Post.PostType", "Invalid post type."));
            postType = parsed;
        }

        var paged = await repository.GetAsync(
            request.Page, request.PageSize, postType, request.Tag, request.AuthorUserId, cancellationToken);

        var dtos = paged.Items.Select(GetPostQueryHandler.ToDto).ToList();
        return new PagedResult<CommunityPostDto>(dtos, paged.TotalCount, paged.Page, paged.PageSize);
    }
}
