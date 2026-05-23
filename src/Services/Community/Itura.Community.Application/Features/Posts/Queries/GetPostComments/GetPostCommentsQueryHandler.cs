using Itura.Community.Application.DTOs;
using Itura.Community.Domain.Entities;
using Itura.Community.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Community.Application.Features.Posts.Queries.GetPostComments;

internal sealed class GetPostCommentsQueryHandler(IPostCommentRepository repository)
    : IRequestHandler<GetPostCommentsQuery, Result<PagedResult<PostCommentDto>>>
{
    public async Task<Result<PagedResult<PostCommentDto>>> Handle(GetPostCommentsQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.GetByPostIdAsync(request.PostId, request.Page, request.PageSize, cancellationToken);
        var dtos = paged.Items.Select(ToDto).ToList();
        return new PagedResult<PostCommentDto>(dtos, paged.TotalCount, paged.Page, paged.PageSize);
    }

    internal static PostCommentDto ToDto(PostComment c) => new(
        c.Id, c.PostId, c.AuthorUserId, c.Body, c.LikeCount, c.CreatedAt);
}
