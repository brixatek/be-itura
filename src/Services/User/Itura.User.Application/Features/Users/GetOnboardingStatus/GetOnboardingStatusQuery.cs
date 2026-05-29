using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.User.Application.Features.Users.GetOnboardingStatus;

public sealed record GetOnboardingStatusQuery(Guid AccountId) : IRequest<Result<OnboardingStatusDto>>;

public sealed record OnboardingStatusDto(
    bool OnboardingCompleted,
    bool AssessmentCompleted,
    List<string> WellnessGoals);
