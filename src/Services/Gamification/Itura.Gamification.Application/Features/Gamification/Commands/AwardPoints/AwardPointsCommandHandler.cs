using Itura.Gamification.Application.Common.Interfaces;
using Itura.Gamification.Application.DTOs;
using Itura.Gamification.Application.Features.Gamification.Queries.GetProfile;
using Itura.Gamification.Domain.Entities;
using Itura.Gamification.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Gamification.Application.Features.Gamification.Commands.AwardPoints;

internal sealed class AwardPointsCommandHandler(
    IUserGamificationProfileRepository repository,
    IGamificationUnitOfWork unitOfWork)
    : IRequestHandler<AwardPointsCommand, Result<GamificationProfileDto>>
{
    public async Task<Result<GamificationProfileDto>> Handle(AwardPointsCommand request, CancellationToken cancellationToken)
    {
        var profile = await repository.GetByUserIdAsync(request.UserId, cancellationToken);

        if (profile is null)
        {
            profile = UserGamificationProfile.Create(request.UserId);
            await repository.AddAsync(profile, cancellationToken);
        }

        var result = profile.AwardPoints(request.Points, request.Reason);
        if (result.IsFailure)
            return Result.Failure<GamificationProfileDto>(result.Error);

        repository.Update(profile);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return GetProfileQueryHandler.ToDto(profile);
    }
}
