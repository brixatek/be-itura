using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.AI.Application.Features.AI.Conversations;

public sealed record DeleteConversationCommand(string ConversationId, Guid UserId) : IRequest<Result>;
