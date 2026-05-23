using Itura.Coach.Application.Common.Interfaces;
using Itura.Coach.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Coach.Application.Features.Coaches.Verification;

internal sealed class SuspendCoachCommandHandler(
    ICoachRepository repository,
    ICoachUnitOfWork unitOfWork)
    : IRequestHandler<SuspendCoachCommand, Result>
{
    public async Task<Result> Handle(SuspendCoachCommand request, CancellationToken cancellationToken)
    {
        var coach = await repository.GetByIdAsync(request.CoachProfileId, cancellationToken);
        if (coach is null)
            return Result.Failure(Error.NotFound("Coach.NotFound", "Coach profile not found."));

        var result = coach.Suspend(request.AdminId, request.Reason);
        if (result.IsFailure) return result;

        repository.Update(coach);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
