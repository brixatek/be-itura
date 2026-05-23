using Itura.Gamification.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Gamification.Application.Features.Gamification.Queries.GetProfile;

public sealed record GetProfileQuery(Guid UserId) : IRequest<Result<GamificationProfileDto>>;
