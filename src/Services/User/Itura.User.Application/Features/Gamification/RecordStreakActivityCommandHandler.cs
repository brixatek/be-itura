using Itura.SharedKernel.Results;
using Itura.User.Application.Common.Interfaces;
using Itura.User.Domain.Entities;
using Itura.User.Domain.Repositories;
using MediatR;

namespace Itura.User.Application.Features.Gamification;

internal sealed class RecordStreakActivityCommandHandler(
    IStreakRepository streakRepository,
    IUserUnitOfWork unitOfWork)
    : IRequestHandler<RecordStreakActivityCommand, Result<StreakResult>>
{
    public async Task<Result<StreakResult>> Handle(RecordStreakActivityCommand request, CancellationToken cancellationToken)
    {
        var streak = await streakRepository.GetAsync(request.UserId, request.StreakType, cancellationToken);

        if (streak is null)
        {
            streak = UserStreak.Create(request.UserId, request.StreakType, request.Date);
            await streakRepository.AddAsync(streak, cancellationToken);
        }
        else
        {
            streak.RecordActivity(request.Date);
            streakRepository.Update(streak);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(new StreakResult(streak.CurrentStreak, streak.LongestStreak));
    }
}
