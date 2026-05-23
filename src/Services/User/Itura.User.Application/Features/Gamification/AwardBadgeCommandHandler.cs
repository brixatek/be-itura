using Itura.SharedKernel.Results;
using Itura.User.Application.Common.Interfaces;
using Itura.User.Domain.Entities;
using Itura.User.Domain.Repositories;
using MediatR;

namespace Itura.User.Application.Features.Gamification;

internal sealed class AwardBadgeCommandHandler(
    IBadgeRepository badgeRepository,
    IUserUnitOfWork unitOfWork)
    : IRequestHandler<AwardBadgeCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(AwardBadgeCommand request, CancellationToken cancellationToken)
    {
        var definition = await badgeRepository.GetDefinitionByNameAsync(request.BadgeName, cancellationToken);
        if (definition is null)
            return Result.Failure<bool>(Error.NotFound("Badge.NotFound", $"Badge '{request.BadgeName}' not found."));

        var alreadyEarned = await badgeRepository.HasEarnedAsync(request.UserId, definition.Id, cancellationToken);
        if (alreadyEarned)
            return Result.Success(false);

        var earned = BadgeEarned.Create(request.UserId, definition.Id);
        await badgeRepository.AddEarnedAsync(earned, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
