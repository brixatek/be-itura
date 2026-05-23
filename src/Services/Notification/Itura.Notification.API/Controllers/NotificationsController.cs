using Itura.Notification.Application.Features.Notifications.GetNotifications;
using Itura.Notification.Application.Features.Notifications.MarkAllRead;
using Itura.Notification.Application.Features.Notifications.MarkRead;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Itura.Notification.API.Controllers;

[ApiController]
[Route("api/v1/notifications")]
[Authorize]
public sealed class NotificationsController(ISender sender) : ControllerBase
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException());

    [HttpGet]
    public async Task<IActionResult> GetNotifications(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetNotificationsQuery(CurrentUserId, page, pageSize), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpPut("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken ct = default)
    {
        var result = await sender.Send(new MarkNotificationReadCommand(id, CurrentUserId), ct);
        if (result.IsFailure) return NotFound(new { error = result.Error.Message });
        return NoContent();
    }

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllRead(CancellationToken ct = default)
    {
        await sender.Send(new MarkAllNotificationsReadCommand(CurrentUserId), ct);
        return NoContent();
    }
}
