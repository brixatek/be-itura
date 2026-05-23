using Itura.Coach.Application.Common.Interfaces;
using Itura.Coach.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Coach.Application.Features.Coaches.DeactivateCoach;

internal sealed class DeactivateCoachCommandHandler(
    ICoachRepository repository,
    ICoachUnitOfWork unitOfWork)
    : IRequestHandler<DeactivateCoachCommand, Result>
{
    public async Task<Result> Handle(DeactivateCoachCommand request, CancellationToken cancellationToken)
    {
        var coach = await repository.GetByIdAsync(request.CoachId, cancellationToken);
        if (coach is null)
            return Result.Failure(Error.NotFound("Coach", request.CoachId));

        if (coach.UserId != request.UserId)
            return Result.Failure(Error.Forbidden());

        coach.Deactivate();
        repository.Update(coach);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
