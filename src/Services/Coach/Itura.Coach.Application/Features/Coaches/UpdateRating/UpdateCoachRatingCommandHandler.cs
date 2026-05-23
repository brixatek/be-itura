using Itura.Coach.Application.Common.Interfaces;
using Itura.Coach.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Coach.Application.Features.Coaches.UpdateRating;

internal sealed class UpdateCoachRatingCommandHandler(
    ICoachRepository repository,
    ICoachUnitOfWork unitOfWork)
    : IRequestHandler<UpdateCoachRatingCommand, Result>
{
    public async Task<Result> Handle(UpdateCoachRatingCommand request, CancellationToken cancellationToken)
    {
        var coach = await repository.GetByIdAsync(request.CoachId, cancellationToken);
        if (coach is null)
            return Result.Failure(Error.NotFound("Coach", request.CoachId));

        coach.UpdateRating(request.NewAverageRating, request.TotalReviews);
        repository.Update(coach);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
