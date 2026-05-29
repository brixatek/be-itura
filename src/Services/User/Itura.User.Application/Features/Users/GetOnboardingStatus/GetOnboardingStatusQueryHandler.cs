using Itura.SharedKernel.Results;
using Itura.User.Domain.Repositories;
using MediatR;

namespace Itura.User.Application.Features.Users.GetOnboardingStatus;

internal sealed class GetOnboardingStatusQueryHandler(
    IUserProfileRepository profileRepository,
    IWellnessAssessmentRepository assessmentRepository)
    : IRequestHandler<GetOnboardingStatusQuery, Result<OnboardingStatusDto>>
{
    public async Task<Result<OnboardingStatusDto>> Handle(
        GetOnboardingStatusQuery request, CancellationToken cancellationToken)
    {
        var profile = await profileRepository.GetByIdAsync(request.AccountId, cancellationToken);
        if (profile is null)
            return Result.Failure<OnboardingStatusDto>(Error.NotFound("UserProfile", request.AccountId));

        var assessment = await assessmentRepository.GetLatestByUserIdAsync(request.AccountId, cancellationToken);

        var dto = new OnboardingStatusDto(
            profile.OnboardingCompleted,
            assessment is not null,
            profile.WellnessGoals.OrderBy(g => g.Priority).Select(g => g.Goal).ToList());

        return Result.Success(dto);
    }
}
