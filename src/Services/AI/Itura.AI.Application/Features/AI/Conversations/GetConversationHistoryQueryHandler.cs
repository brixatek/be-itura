using Itura.AI.Domain.Entities;
using Itura.AI.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.AI.Application.Features.AI.Conversations;

internal sealed class GetConversationHistoryQueryHandler(IConversationRepository conversations)
    : IRequestHandler<GetConversationHistoryQuery, Result<Conversation>>,
      IRequestHandler<ListConversationsQuery, Result<List<ConversationSummary>>>
{
    public async Task<Result<Conversation>> Handle(GetConversationHistoryQuery request, CancellationToken cancellationToken)
    {
        var conv = await conversations.GetByIdAsync(request.ConversationId, cancellationToken);
        if (conv is null || conv.UserId != request.UserId)
            return Result.Failure<Conversation>(Error.NotFound("Conversation.NotFound", "Conversation not found."));

        return Result.Success(conv);
    }

    public async Task<Result<List<ConversationSummary>>> Handle(ListConversationsQuery request, CancellationToken cancellationToken)
    {
        var list = await conversations.GetByUserIdAsync(request.UserId, request.Limit, cancellationToken);
        var summaries = list.Select(c => new ConversationSummary(
            c.Id, c.Title, c.UpdatedAt, c.Messages.Count)).ToList();
        return Result.Success(summaries);
    }
}
