using Itura.Coach.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Coach.Application.Features.Availability;

public sealed record GetAvailableSlotsQuery(
    Guid CoachUserId,
    DateOnly Date,
    string? ViewerTimezone = null) : IRequest<Result<List<TimeSlotDto>>>;
