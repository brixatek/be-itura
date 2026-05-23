using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.User.Application.Features.Gamification;

public sealed record RecordStreakActivityCommand(
    Guid UserId,
    string StreakType,
    DateOnly Date) : IRequest<Result<StreakResult>>;

public sealed record StreakResult(int CurrentStreak, int LongestStreak);
