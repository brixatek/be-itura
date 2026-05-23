using Itura.Community.Application.Common.Interfaces;
using Itura.Community.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Community.Application.Features.Posts.Commands.LikePost;

internal sealed class LikePostCommandHandler(
    ICommunityPostRepository repository,
    ICommunityUnitOfWork unitOfWork)
    : IRequestHandler<LikePostCommand, Result>
{
    public async Task<Result> Handle(LikePostCommand request, CancellationToken cancellationToken)
    {
        var post = await repository.GetByIdAsync(request.PostId, cancellationToken);
        if (post is null)
            return Result.Failure(Error.NotFound("CommunityPost", request.PostId));

        if (request.Unlike) post.Unlike();
        else post.Like();

        repository.Update(post);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
