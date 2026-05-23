using Itura.Coach.Application.Common.Interfaces;
using Itura.Coach.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Coach.Application.Features.Availability;

internal sealed class RemoveAvailabilityCommandHandler(
    IAvailabilityRepository repository,
    ICoachUnitOfWork unitOfWork)
    : IRequestHandler<RemoveAvailabilityCommand, Result>
{
    public async Task<Result> Handle(RemoveAvailabilityCommand request, CancellationToken cancellationToken)
    {
        var block = await repository.GetByIdAsync(request.AvailabilityId, cancellationToken);
        if (block is null)
            return Result.Failure(Error.NotFound("Availability.NotFound", "Availability block not found."));

        if (block.CoachUserId != request.CoachUserId)
            return Result.Failure(Error.Forbidden("Not authorized to remove this availability."));

        repository.Remove(block);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
