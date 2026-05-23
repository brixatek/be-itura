using Itura.Coach.Application.Common.Interfaces;
using Itura.Coach.Application.DTOs;
using Itura.Coach.Domain.Entities;
using Itura.Coach.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Coach.Application.Features.Availability;

internal sealed class AddAvailabilityCommandHandler(
    IAvailabilityRepository repository,
    ICoachUnitOfWork unitOfWork)
    : IRequestHandler<AddAvailabilityCommand, Result<AvailabilityBlockDto>>
{
    public async Task<Result<AvailabilityBlockDto>> Handle(
        AddAvailabilityCommand request, CancellationToken cancellationToken)
    {
        if (request.StartTime >= request.EndTime)
            return Result.Failure<AvailabilityBlockDto>(
                Error.Validation("Availability.InvalidTime", "Start time must be before end time."));

        var block = CoachAvailability.Create(
            request.CoachUserId, request.DayOfWeek,
            request.StartTime, request.EndTime,
            request.SlotDurationMinutes, request.Timezone);

        await repository.AddAsync(block, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new AvailabilityBlockDto(
            block.Id, block.DayOfWeek.ToString(),
            block.StartTime.ToString("HH:mm"), block.EndTime.ToString("HH:mm"),
            block.SlotDurationMinutes, block.Timezone));
    }
}
