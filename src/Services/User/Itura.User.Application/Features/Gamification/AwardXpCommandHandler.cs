using Itura.Contracts.Gamification;
using Itura.SharedKernel.Results;
using Itura.User.Application.Common.Interfaces;
using Itura.User.Domain.Entities;
using Itura.User.Domain.Repositories;
using MassTransit;
using MediatR;

namespace Itura.User.Application.Features.Gamification;

internal sealed class AwardXpCommandHandler(
    IUserProfileRepository profileRepository,
    IXpRepository xpRepository,
    ILeaderboardCache leaderboard,
    IUserUnitOfWork unitOfWork,
    IPublishEndpoint publishEndpoint,
    IBadgeEvaluationService badgeEvaluation)
    : IRequestHandler<AwardXpCommand, Result<AwardXpResult>>
{
    public async Task<Result<AwardXpResult>> Handle(AwardXpCommand request, CancellationToken cancellationToken)
    {
        if (request.Amount <= 0)
            return Result.Failure<AwardXpResult>(
                Error.Validation("Xp.InvalidAmount", "XP amount must be positive."));

        var profile = await profileRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (profile is null)
            return Result.Failure<AwardXpResult>(Error.NotFound("UserProfile", request.UserId));

        var oldLevel = profile.WellnessLevel;
        var newLevel = profile.AwardXp(request.Amount);
        var transaction = XpTransaction.Create(request.UserId, request.Amount, request.Action, request.ReferenceId);

        await xpRepository.AddTransactionAsync(transaction, cancellationToken);
        profileRepository.Update(profile);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await leaderboard.UpdateScoreAsync(request.UserId, profile.TotalXp, cancellationToken);

        await publishEndpoint.Publish(new PointsAwardedEvent(
            request.UserId,
            request.Amount,
            profile.TotalXp,
            profile.WellnessLevel,
            request.Action,
            DateTime.UtcNow), cancellationToken);

        if (newLevel > 0)
        {
            await publishEndpoint.Publish(new LevelUpEvent(
                request.UserId,
                oldLevel,
                newLevel,
                profile.TotalXp,
                DateTime.UtcNow), cancellationToken);
        }

        await badgeEvaluation.EvaluateAndAwardAsync(request.UserId, "xp.awarded", cancellationToken);

        return Result.Success(new AwardXpResult(profile.TotalXp, profile.WellnessLevel, newLevel > 0));
    }
}
