using Itura.Community.Application.DTOs;
using Itura.Community.Domain.Entities;
using Itura.Community.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Community.Application.Features.Posts.Queries.GetPost;

internal sealed class GetPostQueryHandler(ICommunityPostRepository repository)
    : IRequestHandler<GetPostQuery, Result<CommunityPostDto>>
{
    public async Task<Result<CommunityPostDto>> Handle(GetPostQuery request, CancellationToken cancellationToken)
    {
        var post = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (post is null)
            return Result.Failure<CommunityPostDto>(Error.NotFound("CommunityPost", request.Id));

        return ToDto(post);
    }

    internal static CommunityPostDto ToDto(CommunityPost post) => new(
        post.Id,
        post.Title,
        post.Body,
        post.PostType.ToString(),
        post.AuthorUserId,
        post.Tags,
        post.LikeCount,
        post.CommentCount,
        post.IsPublic,
        post.CreatedAt,
        post.UpdatedAt);
}
