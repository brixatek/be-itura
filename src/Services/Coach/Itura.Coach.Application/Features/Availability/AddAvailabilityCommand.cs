using Itura.Coach.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Coach.Application.Features.Availability;

public sealed record AddAvailabilityCommand(
    Guid CoachUserId,
    DayOfWeek DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime,
    int SlotDurationMinutes = 60,
    string Timezone = "UTC") : IRequest<Result<AvailabilityBlockDto>>;
