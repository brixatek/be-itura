using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Coach.Application.Features.Coaches.DeactivateCoach;

public sealed record DeactivateCoachCommand(Guid CoachId, Guid UserId) : IRequest<Result>;
