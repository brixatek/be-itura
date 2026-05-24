using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Booking.Application.Features.Bookings.GetCoachAvailability;

public sealed record GetCoachAvailabilityQuery(Guid CoachUserId, DateOnly Date) : IRequest<Result<CoachAvailabilityDto>>;

public sealed record CoachAvailabilityDto(Guid CoachUserId, DateOnly Date, List<DateTime> BookedSlots);
