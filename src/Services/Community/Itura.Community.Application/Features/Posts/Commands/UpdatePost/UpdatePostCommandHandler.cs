using Itura.Community.Application.Common.Interfaces;
using Itura.Community.Application.DTOs;
using Itura.Community.Application.Features.Posts.Queries.GetPost;
using Itura.Community.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Community.Application.Features.Posts.Commands.UpdatePost;

internal sealed class UpdatePostCommandHandler(
    ICommunityPostRepository repository,
    ICommunityUnitOfWork unitOfWork)
    : IRequestHandler<UpdatePostCommand, Result<CommunityPostDto>>
{
    public async Task<Result<CommunityPostDto>> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var post = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (post is null)
            return Result.Failure<CommunityPostDto>(Error.NotFound("CommunityPost", request.Id));

        if (post.AuthorUserId != request.AuthorUserId)
            return Result.Failure<CommunityPostDto>(Error.Forbidden("Only the author can update this post."));

        var result = post.Update(request.Body, request.Title, request.Tags);
        if (result.IsFailure)
            return Result.Failure<CommunityPostDto>(result.Error);

        repository.Update(post);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return GetPostQueryHandler.ToDto(post);
    }
}
