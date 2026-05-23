using Itura.Community.Application.Common.Interfaces;
using Itura.Community.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Community.Application.Features.Posts.Commands.DeletePost;

internal sealed class DeletePostCommandHandler(
    ICommunityPostRepository repository,
    ICommunityUnitOfWork unitOfWork)
    : IRequestHandler<DeletePostCommand, Result>
{
    public async Task<Result> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var post = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (post is null)
            return Result.Failure(Error.NotFound("CommunityPost", request.Id));

        if (post.AuthorUserId != request.AuthorUserId)
            return Result.Failure(Error.Forbidden("Only the author can delete this post."));

        post.Delete();
        repository.Update(post);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
