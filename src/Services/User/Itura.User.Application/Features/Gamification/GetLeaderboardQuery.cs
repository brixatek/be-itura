using Itura.SharedKernel.Results;
using Itura.User.Application.Common.Interfaces;
using MediatR;

namespace Itura.User.Application.Features.Gamification;

public sealed record GetLeaderboardQuery(int Count = 50) : IRequest<Result<List<LeaderboardEntry>>>;
