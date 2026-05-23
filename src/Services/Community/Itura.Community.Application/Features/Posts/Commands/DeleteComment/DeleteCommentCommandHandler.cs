using Itura.Community.Application.Common.Interfaces;
using Itura.Community.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Community.Application.Features.Posts.Commands.DeleteComment;

internal sealed class DeleteCommentCommandHandler(
    ICommunityPostRepository postRepository,
    IPostCommentRepository commentRepository,
    ICommunityUnitOfWork unitOfWork)
    : IRequestHandler<DeleteCommentCommand, Result>
{
    public async Task<Result> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await commentRepository.GetByIdAsync(request.CommentId, cancellationToken);
        if (comment is null)
            return Result.Failure(Error.NotFound("PostComment", request.CommentId));

        if (comment.AuthorUserId != request.AuthorUserId)
            return Result.Failure(Error.Forbidden("Only the author can delete this comment."));

        var post = await postRepository.GetByIdAsync(comment.PostId, cancellationToken);
        if (post is not null)
        {
            post.DecrementComments();
            postRepository.Update(post);
        }

        comment.Delete();
        commentRepository.Update(comment);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
