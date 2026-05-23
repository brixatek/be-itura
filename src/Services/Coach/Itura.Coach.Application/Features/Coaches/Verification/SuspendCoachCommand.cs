using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Coach.Application.Features.Coaches.Verification;

public sealed record SuspendCoachCommand(Guid CoachProfileId, Guid AdminId, string Reason) : IRequest<Result>;
