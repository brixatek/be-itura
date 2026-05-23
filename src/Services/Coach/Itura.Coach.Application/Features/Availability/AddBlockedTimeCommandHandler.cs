using Itura.Coach.Application.Common.Interfaces;
using Itura.Coach.Domain.Entities;
using Itura.Coach.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Coach.Application.Features.Availability;

internal sealed class AddBlockedTimeCommandHandler(
    IAvailabilityRepository repository,
    ICoachUnitOfWork unitOfWork)
    : IRequestHandler<AddBlockedTimeCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddBlockedTimeCommand request, CancellationToken cancellationToken)
    {
        if (request.StartUtc >= request.EndUtc)
            return Result.Failure<Guid>(
                Error.Validation("BlockedTime.InvalidRange", "Start must be before end."));

        var blocked = CoachBlockedTime.Create(
            request.CoachUserId, request.StartUtc, request.EndUtc, request.Reason);

        await repository.AddBlockedTimeAsync(blocked, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(blocked.Id);
    }
}
