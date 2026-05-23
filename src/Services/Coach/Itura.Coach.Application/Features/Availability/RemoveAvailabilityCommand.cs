using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Coach.Application.Features.Availability;

public sealed record RemoveAvailabilityCommand(Guid AvailabilityId, Guid CoachUserId) : IRequest<Result>;
