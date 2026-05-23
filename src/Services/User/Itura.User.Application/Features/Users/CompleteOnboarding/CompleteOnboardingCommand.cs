using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.User.Application.Features.Users.CompleteOnboarding;

public sealed record CompleteOnboardingCommand(
    Guid AccountId,
    List<string> WellnessGoals) : IRequest<Result>;
