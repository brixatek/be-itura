using Itura.Analytics.Application.DTOs;
using Itura.Analytics.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Analytics.Application.Features.Analytics.Queries.GetUserEvents;

internal sealed class GetUserEventsQueryHandler(IAnalyticsEventRepository repository)
    : IRequestHandler<GetUserEventsQuery, Result<PagedResult<AnalyticsEventDto>>>
{
    public async Task<Result<PagedResult<AnalyticsEventDto>>> Handle(GetUserEventsQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Min(Math.Max(request.PageSize, 1), 100);
        var (items, total) = await repository.GetUserEventsAsync(request.UserId, page, pageSize, cancellationToken);
        var dtos = items.Select(e => new AnalyticsEventDto(e.Id, e.UserId, e.EventType, e.Source, e.PropertiesJson, e.OccurredAt));
        return new PagedResult<AnalyticsEventDto>(dtos, total, page, pageSize);
    }
}
