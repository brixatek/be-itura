using Itura.SharedKernel.Results;
using Itura.User.Application.Common.Interfaces;
using Itura.User.Domain.Repositories;
using MediatR;

namespace Itura.User.Application.Features.Users.CompleteOnboarding;

internal sealed class CompleteOnboardingCommandHandler(
    IUserProfileRepository profileRepository,
    IUserUnitOfWork unitOfWork) : IRequestHandler<CompleteOnboardingCommand, Result>
{
    public async Task<Result> Handle(CompleteOnboardingCommand request, CancellationToken cancellationToken)
    {
        var profile = await profileRepository.GetByIdAsync(request.AccountId, cancellationToken);
        if (profile is null)
            return Result.Failure(Error.NotFound("UserProfile", request.AccountId));

        var result = profile.CompleteOnboarding(request.WellnessGoals);
        if (result.IsFailure) return result;

        profileRepository.Update(profile);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
