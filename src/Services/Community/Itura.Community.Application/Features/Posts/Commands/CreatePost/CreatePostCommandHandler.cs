using Itura.Community.Application.Common.Interfaces;
using Itura.Community.Application.DTOs;
using Itura.Community.Application.Features.Posts.Queries.GetPost;
using Itura.Community.Domain.Enums;
using Itura.Community.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Community.Application.Features.Posts.Commands.CreatePost;

internal sealed class CreatePostCommandHandler(
    ICommunityPostRepository repository,
    ICommunityUnitOfWork unitOfWork,
    IContentModerationService moderation)
    : IRequestHandler<CreatePostCommand, Result<CommunityPostDto>>
{
    public async Task<Result<CommunityPostDto>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<PostType>(request.PostType, true, out var postType))
            return Result.Failure<CommunityPostDto>(Error.Validation("Post.PostType", "Invalid post type."));

        var modResult = await moderation.CheckAsync(
            $"{request.Title} {request.Body}", cancellationToken);
        if (modResult.IsFlagged)
            return Result.Failure<CommunityPostDto>(
                Error.Validation("Post.Content", modResult.Reason ?? "Content violates community guidelines."));

        var result = Domain.Entities.CommunityPost.Create(
            request.Body, postType, request.AuthorUserId,
            request.Title, request.Tags, request.IsPublic, request.IsAnonymous);

        if (result.IsFailure)
            return Result.Failure<CommunityPostDto>(result.Error);

        await repository.AddAsync(result.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return GetPostQueryHandler.ToDto(result.Value);
    }
}
