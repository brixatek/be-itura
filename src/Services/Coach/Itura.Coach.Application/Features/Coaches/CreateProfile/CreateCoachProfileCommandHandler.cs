using Itura.Coach.Application.Common.Interfaces;
using Itura.Coach.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Coach.Application.Features.Coaches.CreateProfile;

internal sealed class CreateCoachProfileCommandHandler(
    ICoachRepository repository,
    ICoachUnitOfWork unitOfWork)
    : IRequestHandler<CreateCoachProfileCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateCoachProfileCommand request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (existing is not null)
            return Result.Failure<Guid>(new Error("Coach.AlreadyExists", "A coach profile already exists for this user."));

        var result = Domain.Entities.CoachProfile.Create(
            request.UserId, request.Email, request.DisplayName, request.Bio,
            request.Specializations, request.Languages,
            request.HourlyRate, request.Currency,
            request.ProfileImageUrl, request.YearsOfExperience);

        if (result.IsFailure) return Result.Failure<Guid>(result.Error);

        await repository.AddAsync(result.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(result.Value.Id);
    }
}
