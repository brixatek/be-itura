using Itura.Coach.Application.DTOs;
using Itura.Coach.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Coach.Application.Features.Availability;

internal sealed class GetAvailabilityQueryHandler(IAvailabilityRepository repository)
    : IRequestHandler<GetAvailabilityQuery, Result<List<AvailabilityBlockDto>>>
{
    public async Task<Result<List<AvailabilityBlockDto>>> Handle(
        GetAvailabilityQuery request, CancellationToken cancellationToken)
    {
        var items = await repository.GetByCoachUserIdAsync(request.CoachUserId, cancellationToken);
        var dtos = items.Select(a => new AvailabilityBlockDto(
            a.Id, a.DayOfWeek.ToString(),
            a.StartTime.ToString("HH:mm"), a.EndTime.ToString("HH:mm"),
            a.SlotDurationMinutes, a.Timezone)).ToList();
        return Result.Success(dtos);
    }
}
