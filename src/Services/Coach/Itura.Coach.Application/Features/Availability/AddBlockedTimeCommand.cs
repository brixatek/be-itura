using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Coach.Application.Features.Availability;

public sealed record AddBlockedTimeCommand(
    Guid CoachUserId,
    DateTime StartUtc,
    DateTime EndUtc,
    string? Reason = null) : IRequest<Result<Guid>>;
