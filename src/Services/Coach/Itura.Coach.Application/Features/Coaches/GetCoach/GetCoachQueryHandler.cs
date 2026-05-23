using Itura.Coach.Application.DTOs;
using Itura.Coach.Domain.Entities;
using Itura.Coach.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Coach.Application.Features.Coaches.GetCoach;

internal sealed class GetCoachQueryHandler(ICoachRepository repository)
    : IRequestHandler<GetCoachQuery, Result<CoachDto>>
{
    public async Task<Result<CoachDto>> Handle(GetCoachQuery request, CancellationToken cancellationToken)
    {
        var coach = await repository.GetByIdAsync(request.CoachId, cancellationToken);
        if (coach is null)
            return Result.Failure<CoachDto>(Error.NotFound("Coach", request.CoachId));

        return Result.Success(ToDto(coach));
    }

    internal static CoachDto ToDto(CoachProfile c) => new(
        c.Id, c.UserId, c.DisplayName, c.Bio,
        c.Specializations, c.Languages,
        c.HourlyRate, c.Currency, c.ProfileImageUrl,
        c.YearsOfExperience, c.IsActive,
        c.AverageRating, c.TotalReviews,
        c.VerificationStatus.ToString(), c.VerifiedAt, c.RejectionReason,
        c.CreatedAt, c.UpdatedAt);
}
