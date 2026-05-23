using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Mood.Application.Features.Mood.LogMood;

public sealed record LogMoodCommand(
    Guid UserId,
    int Score,
    string? Note,
    IReadOnlyList<string> Emotions,
    DateTime? LoggedAt = null) : IRequest<Result<Guid>>;
