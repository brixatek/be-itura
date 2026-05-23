using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.User.Application.Features.Gamification;

public sealed record GetBadgesQuery(Guid UserId) : IRequest<Result<List<BadgeDto>>>;

public sealed record BadgeDto(Guid Id, string Name, string Description, string IconUrl, DateTime EarnedAt);
