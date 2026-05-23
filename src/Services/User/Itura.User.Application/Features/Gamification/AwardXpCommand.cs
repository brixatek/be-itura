using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.User.Application.Features.Gamification;

public sealed record AwardXpCommand(
    Guid UserId,
    int Amount,
    string Action,
    Guid? ReferenceId = null) : IRequest<Result<AwardXpResult>>;

public sealed record AwardXpResult(int NewTotal, int NewLevel, bool LeveledUp);
