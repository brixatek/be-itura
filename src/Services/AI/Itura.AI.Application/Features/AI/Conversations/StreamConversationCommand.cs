using MediatR;

namespace Itura.AI.Application.Features.AI.Conversations;

public sealed record StreamConversationCommand(
    Guid UserId,
    string? ConversationId,
    string Message,
    string Tier = "free") : IStreamRequest<string>;
