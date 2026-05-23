using Itura.Analytics.Application.Common.Interfaces;
using Itura.Analytics.Application.DTOs;
using Itura.Analytics.Domain.Entities;
using Itura.Analytics.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Analytics.Application.Features.Analytics.Commands.TrackEvent;

internal sealed class TrackEventCommandHandler(
    IAnalyticsEventRepository repository,
    IAnalyticsUnitOfWork unitOfWork)
    : IRequestHandler<TrackEventCommand, Result<AnalyticsEventDto>>
{
    public async Task<Result<AnalyticsEventDto>> Handle(TrackEventCommand request, CancellationToken cancellationToken)
    {
        var ev = AnalyticsEvent.Create(request.UserId, request.EventType, request.Source, request.PropertiesJson);
        await repository.AddAsync(ev, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new AnalyticsEventDto(ev.Id, ev.UserId, ev.EventType, ev.Source, ev.PropertiesJson, ev.OccurredAt);
    }
}
