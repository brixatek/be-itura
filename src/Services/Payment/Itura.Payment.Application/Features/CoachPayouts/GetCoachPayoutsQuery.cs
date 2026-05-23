using Itura.Payment.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Payment.Application.Features.CoachPayouts;

public sealed record GetCoachPayoutsQuery(
    Guid CoachUserId,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<PagedResult<CoachPayoutDto>>>;
