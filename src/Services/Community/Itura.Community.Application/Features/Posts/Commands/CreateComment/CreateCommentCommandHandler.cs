using Itura.Community.Application.Common.Interfaces;
using Itura.Community.Application.DTOs;
using Itura.Community.Application.Features.Posts.Queries.GetPostComments;
using Itura.Community.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Community.Application.Features.Posts.Commands.CreateComment;

internal sealed class CreateCommentCommandHandler(
    ICommunityPostRepository postRepository,
    IPostCommentRepository commentRepository,
    ICommunityUnitOfWork unitOfWork)
    : IRequestHandler<CreateCommentCommand, Result<PostCommentDto>>
{
    public async Task<Result<PostCommentDto>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var post = await postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post is null)
            return Result.Failure<PostCommentDto>(Error.NotFound("CommunityPost", request.PostId));

        var result = Domain.Entities.PostComment.Create(request.PostId, request.AuthorUserId, request.Body);
        if (result.IsFailure)
            return Result.Failure<PostCommentDto>(result.Error);

        post.IncrementComments();
        postRepository.Update(post);

        await commentRepository.AddAsync(result.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return GetPostCommentsQueryHandler.ToDto(result.Value);
    }
}
