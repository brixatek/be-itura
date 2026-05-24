using Itura.Community.Application.Common.Interfaces;
using Itura.Community.Domain.Entities;
using Itura.Community.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Community.Application.Features.Posts.Commands.ReactToPost;

internal sealed class ReactToPostCommandHandler(
    ICommunityPostRepository postRepository,
    IPostReactionRepository reactionRepository,
    ICommunityUnitOfWork unitOfWork)
    : IRequestHandler<ReactToPostCommand, Result>
{
    private static readonly HashSet<string> AllowedEmojis =
        ["👍", "❤️", "😂", "😮", "😢", "🔥", "👏", "🙏"];

    public async Task<Result> Handle(ReactToPostCommand request, CancellationToken cancellationToken)
    {
        if (!AllowedEmojis.Contains(request.Emoji))
            return Result.Failure(Error.Validation("Reaction.Emoji", "Emoji not supported."));

        var post = await postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post is null)
            return Result.Failure(Error.NotFound("CommunityPost", request.PostId));

        var existing = await reactionRepository.GetAsync(request.PostId, request.UserId, request.Emoji, cancellationToken);

        if (request.Remove)
        {
            if (existing is null)
                return Result.Failure(Error.NotFound("Reaction.NotFound", "Reaction not found."));
            reactionRepository.Remove(existing);
        }
        else
        {
            if (existing is not null)
                return Result.Failure(Error.Conflict("Reaction.AlreadyExists", "You have already reacted with this emoji."));
            var reaction = PostReaction.Create(request.PostId, request.UserId, request.Emoji);
            await reactionRepository.AddAsync(reaction, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
