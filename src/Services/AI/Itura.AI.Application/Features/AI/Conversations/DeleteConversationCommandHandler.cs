using Itura.AI.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.AI.Application.Features.AI.Conversations;

internal sealed class DeleteConversationCommandHandler(IConversationRepository conversations)
    : IRequestHandler<DeleteConversationCommand, Result>
{
    public async Task<Result> Handle(DeleteConversationCommand request, CancellationToken cancellationToken)
    {
        var conversation = await conversations.GetByIdAsync(request.ConversationId, cancellationToken);
        if (conversation is null)
            return Result.Failure(Error.NotFound("Conversation.NotFound", "Conversation not found."));

        if (conversation.UserId != request.UserId)
            return Result.Failure(Error.Forbidden());

        await conversations.DeleteAsync(request.ConversationId, cancellationToken);
        return Result.Success();
    }
}
