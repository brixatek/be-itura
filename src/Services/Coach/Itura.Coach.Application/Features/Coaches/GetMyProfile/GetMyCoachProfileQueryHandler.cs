using Itura.Coach.Application.DTOs;
using Itura.Coach.Application.Features.Coaches.GetCoach;
using Itura.Coach.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Coach.Application.Features.Coaches.GetMyProfile;

internal sealed class GetMyCoachProfileQueryHandler(ICoachRepository repository)
    : IRequestHandler<GetMyCoachProfileQuery, Result<CoachDto>>
{
    public async Task<Result<CoachDto>> Handle(GetMyCoachProfileQuery request, CancellationToken cancellationToken)
    {
        var coach = await repository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (coach is null)
            return Result.Failure<CoachDto>(new Error("Coach.NotFound", "No coach profile found for this user."));

        return Result.Success(GetCoachQueryHandler.ToDto(coach));
    }
}
