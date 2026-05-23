using Itura.AI.Domain.Entities;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.AI.Application.Features.AI.Conversations;

public sealed record GetConversationHistoryQuery(string ConversationId, Guid UserId)
    : IRequest<Result<Conversation>>;

public sealed record ListConversationsQuery(Guid UserId, int Limit = 20)
    : IRequest<Result<List<ConversationSummary>>>;

public sealed record ConversationSummary(string Id, string Title, DateTime UpdatedAt, int MessageCount);
