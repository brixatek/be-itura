using Itura.Gamification.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Gamification.Application.Features.Gamification.Queries.GetLeaderboard;

public sealed record GetLeaderboardQuery(int Top = 10) : IRequest<Result<PagedResult<GamificationProfileDto>>>;
