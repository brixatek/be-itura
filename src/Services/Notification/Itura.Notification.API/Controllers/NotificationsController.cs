using Itura.Notification.Application.Features.Notifications.DeviceTokens;
using Itura.Notification.Application.Features.Notifications.GetNotifications;
using Itura.Notification.Application.Features.Notifications.MarkAllRead;
using Itura.Notification.Application.Features.Notifications.MarkRead;
using Itura.Notification.Application.Features.Notifications.UpdatePreferences;
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

    // ─── Device tokens ────────────────────────────────────────────────────────

    [HttpPost("device-tokens")]
    public async Task<IActionResult> RegisterDeviceToken([FromBody] DeviceTokenRequest request, CancellationToken ct = default)
    {
        var result = await sender.Send(new RegisterDeviceTokenCommand(CurrentUserId, request.Token, request.Platform), ct);
        return result.IsSuccess ? Ok(new { success = true }) : BadRequest(new { error = result.Error.Message });
    }

    [HttpDelete("device-tokens/{token}")]
    public async Task<IActionResult> UnregisterDeviceToken(string token, CancellationToken ct = default)
    {
        var result = await sender.Send(new UnregisterDeviceTokenCommand(CurrentUserId, token), ct);
        return result.IsSuccess ? NoContent() : NotFound(new { error = result.Error.Message });
    }

    // ─── Preferences ─────────────────────────────────────────────────────────

    [HttpPut("preferences")]
    public async Task<IActionResult> UpdatePreferences(
        [FromBody] UpdatePreferencesRequest request,
        CancellationToken ct = default)
    {
        TimeOnly? quietStart = request.QuietHoursStart.HasValue
            ? TimeOnly.FromTimeSpan(request.QuietHoursStart.Value)
            : null;
        TimeOnly? quietEnd = request.QuietHoursEnd.HasValue
            ? TimeOnly.FromTimeSpan(request.QuietHoursEnd.Value)
            : null;

        var result = await sender.Send(new UpdateNotificationPreferencesCommand(
            CurrentUserId, request.PushEnabled, request.EmailEnabled, quietStart, quietEnd), ct);

        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error.Message });
    }
}

public sealed record DeviceTokenRequest(string Token, string Platform);

public sealed record UpdatePreferencesRequest(
    bool PushEnabled,
    bool EmailEnabled,
    TimeSpan? QuietHoursStart,
    TimeSpan? QuietHoursEnd);
