using Itura.Coach.Application.Common.Interfaces;
using Itura.Coach.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Coach.Application.Features.Coaches.UpdateProfile;

internal sealed class UpdateCoachProfileCommandHandler(
    ICoachRepository repository,
    ICoachUnitOfWork unitOfWork)
    : IRequestHandler<UpdateCoachProfileCommand, Result>
{
    public async Task<Result> Handle(UpdateCoachProfileCommand request, CancellationToken cancellationToken)
    {
        var coach = await repository.GetByIdAsync(request.CoachId, cancellationToken);
        if (coach is null)
            return Result.Failure(Error.NotFound("Coach", request.CoachId));

        if (coach.UserId != request.UserId)
            return Result.Failure(Error.Forbidden());

        var result = coach.UpdateProfile(
            request.DisplayName, request.Bio,
            request.Specializations, request.Languages,
            request.HourlyRate, request.Currency,
            request.ProfileImageUrl, request.YearsOfExperience);

        if (result.IsFailure) return result;

        repository.Update(coach);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
