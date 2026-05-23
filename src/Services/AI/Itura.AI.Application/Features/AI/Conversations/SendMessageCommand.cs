using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.AI.Application.Features.AI.Conversations;

public sealed record SendMessageCommand(
    Guid UserId,
    string? ConversationId,
    string Message) : IRequest<Result<SendMessageResult>>;

public sealed record SendMessageResult(
    string ConversationId,
    string Reply,
    bool CrisisDetected);
