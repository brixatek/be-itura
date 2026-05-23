using Itura.Coach.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Coach.Application.Features.Coaches.Verification;

public sealed record GetPendingCoachesQuery(int Page = 1, int PageSize = 20) : IRequest<Result<PagedResult<CoachDto>>>;
