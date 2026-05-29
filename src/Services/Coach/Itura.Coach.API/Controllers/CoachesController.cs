using Itura.Coach.Application.Features.Coaches.CreateProfile;
using Itura.Coach.Application.Features.Coaches.DeactivateCoach;
using Itura.Coach.Application.Features.Coaches.GetCoach;
using Itura.Coach.Application.Features.Coaches.GetCoaches;
using Itura.Coach.Application.Features.Coaches.GetMyProfile;
using Itura.Coach.Application.Features.Coaches.UpdateProfile;
using Itura.Coach.Application.Features.Availability;
using Itura.Coach.Application.Features.Coaches.Verification;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Itura.Coach.API.Controllers;

[ApiController]
[Route("api/v1/coaches")]
[Authorize]
public sealed class CoachesController(ISender sender) : ControllerBase
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException());

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetCoaches(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? specialization = null,
        [FromQuery] string? language = null,
        [FromQuery] double? minRating = null,
        [FromQuery] decimal? maxHourlyRate = null,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetCoachesQuery(page, pageSize, specialization, language, minRating, maxHourlyRate), ct);
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCoach(Guid id, CancellationToken ct = default)
    {
        var result = await sender.Send(new GetCoachQuery(id), ct);
        if (result.IsFailure) return NotFound(new { error = result.Error.Message });
        return Ok(result.Value);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile(CancellationToken ct = default)
    {
        var result = await sender.Send(new GetMyCoachProfileQuery(CurrentUserId), ct);
        if (result.IsFailure) return NotFound(new { error = result.Error.Message });
        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProfile([FromBody] CreateCoachProfileRequest request, CancellationToken ct = default)
    {
        var email = User.FindFirstValue(System.Security.Claims.ClaimTypes.Email)
            ?? User.FindFirstValue("email")
            ?? string.Empty;

        var result = await sender.Send(new CreateCoachProfileCommand(
            CurrentUserId, email, request.DisplayName, request.Bio ?? string.Empty,
            request.Specializations ?? [], request.Languages ?? ["english"],
            request.HourlyRate, request.Currency ?? "USD",
            request.ProfileImageUrl, request.YearsOfExperience), ct);

        if (result.IsFailure) return BadRequest(new { error = result.Error.Message });
        return CreatedAtAction(nameof(GetCoach), new { id = result.Value }, new { id = result.Value });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] UpdateCoachProfileRequest request, CancellationToken ct = default)
    {
        var result = await sender.Send(new UpdateCoachProfileCommand(
            id, CurrentUserId, request.DisplayName, request.Bio ?? string.Empty,
            request.Specializations ?? [], request.Languages ?? ["english"],
            request.HourlyRate, request.Currency ?? "USD",
            request.ProfileImageUrl, request.YearsOfExperience), ct);

        if (result.IsFailure) return result.Error.Code.StartsWith("Auth.")
            ? Forbid()
            : NotFound(new { error = result.Error.Message });

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct = default)
    {
        var result = await sender.Send(new DeactivateCoachCommand(id, CurrentUserId), ct);
        if (result.IsFailure) return result.Error.Code.StartsWith("Auth.")
            ? Forbid()
            : NotFound(new { error = result.Error.Message });

        return NoContent();
    }
}

public sealed record CreateCoachProfileRequest(
    string DisplayName,
    string? Bio,
    IReadOnlyList<string>? Specializations,
    IReadOnlyList<string>? Languages,
    decimal HourlyRate,
    string? Currency,
    string? ProfileImageUrl,
    int YearsOfExperience);

public sealed record UpdateCoachProfileRequest(
    string DisplayName,
    string? Bio,
    IReadOnlyList<string>? Specializations,
    IReadOnlyList<string>? Languages,
    decimal HourlyRate,
    string? Currency,
    string? ProfileImageUrl,
    int YearsOfExperience);

// ─── Admin verification controller ───────────────────────────────────────────

[ApiController]
[Route("api/v1/admin/coaches")]
[Authorize(Roles = "Admin")]
public sealed class AdminCoachesController(ISender sender) : ControllerBase
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException());

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await sender.Send(new GetPendingCoachesQuery(page, pageSize), ct);
        return Ok(new { success = true, data = result.Value });
    }

    [HttpGet("{id:guid}/review")]
    public async Task<IActionResult> Review(Guid id, CancellationToken ct = default)
    {
        var result = await sender.Send(new GetCoachQuery(id), ct);
        if (result.IsFailure) return NotFound(new { error = result.Error.Message });
        return Ok(new { success = true, data = result.Value });
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id, CancellationToken ct = default)
    {
        var result = await sender.Send(new ApproveCoachCommand(id, CurrentUserId), ct);
        return result.IsSuccess ? Ok(new { success = true }) : BadRequest(new { error = result.Error });
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectRequest request, CancellationToken ct = default)
    {
        var result = await sender.Send(new RejectCoachCommand(id, CurrentUserId, request.Reason), ct);
        return result.IsSuccess ? Ok(new { success = true }) : BadRequest(new { error = result.Error });
    }

    [HttpPost("{id:guid}/suspend")]
    public async Task<IActionResult> Suspend(Guid id, [FromBody] RejectRequest request, CancellationToken ct = default)
    {
        var result = await sender.Send(new SuspendCoachCommand(id, CurrentUserId, request.Reason), ct);
        return result.IsSuccess ? Ok(new { success = true }) : BadRequest(new { error = result.Error });
    }
}

public sealed record RejectRequest(string Reason);

// ─── Availability controller ──────────────────────────────────────────────────

[ApiController]
[Route("api/v1/coaches")]
[Authorize]
public sealed class AvailabilityController(ISender sender) : ControllerBase
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException());

    [HttpGet("me/availability")]
    public async Task<IActionResult> GetMyAvailability(CancellationToken ct)
    {
        var result = await sender.Send(new GetAvailabilityQuery(CurrentUserId), ct);
        return Ok(new { success = true, data = result.Value });
    }

    [HttpPost("me/availability")]
    public async Task<IActionResult> AddAvailability([FromBody] AddAvailabilityRequest request, CancellationToken ct)
    {
        if (!TimeOnly.TryParse(request.StartTime, out var start) ||
            !TimeOnly.TryParse(request.EndTime, out var end))
            return BadRequest(new { error = "Invalid time format. Use HH:mm." });

        var result = await sender.Send(new AddAvailabilityCommand(
            CurrentUserId, request.DayOfWeek, start, end,
            request.SlotDurationMinutes, request.Timezone ?? "UTC"), ct);

        return result.IsSuccess
            ? Ok(new { success = true, data = result.Value })
            : BadRequest(new { error = result.Error });
    }

    [HttpDelete("me/availability/{id:guid}")]
    public async Task<IActionResult> RemoveAvailability(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new RemoveAvailabilityCommand(id, CurrentUserId), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.Contains("NotFound")) return NotFound(new { error = result.Error });
            if (result.Error.Code.Contains("Forbidden")) return Forbid();
            return BadRequest(new { error = result.Error });
        }
        return NoContent();
    }

    [HttpGet("{coachUserId:guid}/available-slots")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAvailableSlots(
        Guid coachUserId,
        [FromQuery] string date,
        [FromQuery] string? timezone = null,
        CancellationToken ct = default)
    {
        if (!DateOnly.TryParse(date, out var parsedDate))
            return BadRequest(new { error = "Invalid date format. Use yyyy-MM-dd." });

        var result = await sender.Send(new GetAvailableSlotsQuery(coachUserId, parsedDate, timezone), ct);
        return Ok(new { success = true, data = result.Value });
    }

    [HttpPost("me/blocked-times")]
    public async Task<IActionResult> AddBlockedTime([FromBody] AddBlockedTimeRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new AddBlockedTimeCommand(
            CurrentUserId, request.StartUtc, request.EndUtc, request.Reason), ct);
        return result.IsSuccess
            ? Ok(new { success = true, data = new { id = result.Value } })
            : BadRequest(new { error = result.Error });
    }
}

public sealed record AddAvailabilityRequest(
    DayOfWeek DayOfWeek,
    string StartTime,
    string EndTime,
    int SlotDurationMinutes = 60,
    string? Timezone = null);

public sealed record AddBlockedTimeRequest(
    DateTime StartUtc,
    DateTime EndUtc,
    string? Reason = null);
