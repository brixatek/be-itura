using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Coach.Application.Features.Coaches.Verification;

public sealed record ApproveCoachCommand(Guid CoachProfileId, Guid AdminId) : IRequest<Result>;
