using Itura.Analytics.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Analytics.Application.Features.Analytics.Queries.GetUserEvents;

public sealed record GetUserEventsQuery(
    Guid UserId,
    int Page,
    int PageSize) : IRequest<Result<PagedResult<AnalyticsEventDto>>>;
