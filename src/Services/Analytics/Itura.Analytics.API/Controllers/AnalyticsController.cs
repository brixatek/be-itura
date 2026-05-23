using Itura.Analytics.Application.Features.Analytics.Commands.TrackEvent;
using Itura.Analytics.Application.Features.Analytics.Queries.GetUserEvents;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Itura.Analytics.API.Controllers;

[ApiController]
[Route("api/v1/analytics")]
public sealed class AnalyticsController(IMediator mediator) : ControllerBase
{
    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost("track")]
    [AllowAnonymous]
    public async Task<IActionResult> Track([FromBody] TrackEventRequest request, CancellationToken ct)
    {
        Guid? userId = User.Identity?.IsAuthenticated == true ? GetUserId() : null;
        var command = new TrackEventCommand(userId, request.EventType, request.Source, request.PropertiesJson);
        var result = await mediator.Send(command, ct);
        if (result.IsFailure)
            return BadRequest(new { success = false, error = result.Error });
        return Ok(new { success = true, data = result.Value });
    }

    [HttpGet("events/me")]
    [Authorize]
    public async Task<IActionResult> GetMyEvents(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetUserEventsQuery(GetUserId(), page, pageSize), ct);
        return Ok(new { success = true, data = result.Value });
    }

    [HttpGet("events/{userId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetUserEvents(
        Guid userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetUserEventsQuery(userId, page, pageSize), ct);
        return Ok(new { success = true, data = result.Value });
    }
}

public sealed record TrackEventRequest(string EventType, string Source, string? PropertiesJson);
