using Itura.Analytics.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Analytics.Application.Features.Analytics.Commands.TrackEvent;

public sealed record TrackEventCommand(
    Guid? UserId,
    string EventType,
    string Source,
    string? PropertiesJson) : IRequest<Result<AnalyticsEventDto>>;
