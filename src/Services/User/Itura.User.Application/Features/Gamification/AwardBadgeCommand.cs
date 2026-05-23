using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.User.Application.Features.Gamification;

public sealed record AwardBadgeCommand(Guid UserId, string BadgeName) : IRequest<Result<bool>>;
